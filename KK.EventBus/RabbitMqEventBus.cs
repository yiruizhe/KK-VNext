using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace KK.EventBus;

public class RabbitMqEventBus : IEventBus, IDisposable
{
    private IModel _consumerChannel;
    private readonly string _exchangeName;
    private string _queueName;
    private readonly RabbitMqConnection _persistentConnetion;
    private readonly SubscritionsManager _subsManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScope _serviceScope;

    public RabbitMqEventBus(string exchangeName, string queueName,
        RabbitMqConnection persistentConnetion, IServiceScopeFactory serviceScopeFactory)
    {
        _exchangeName = exchangeName;
        _subsManager = new();
        _queueName = queueName;
        _persistentConnetion = persistentConnetion ?? throw new ArgumentNullException(nameof(persistentConnetion));

        //因为RabbitMQEventBus是Singleton对象，而它创建的IIntegrationEventHandler
        //以及用到的IIntegrationEventHandler用到的服务
        //大部分是Scoped，因此必须这样显式创建一个scope，否则在getservice的时候会报错：
        //Cannot resolvefrom root provider because it requires scoped service
        this._serviceScope = serviceScopeFactory.CreateScope();
        _serviceProvider = _serviceScope.ServiceProvider;
        _consumerChannel = CreateConsumerChannel();
        _subsManager.OnEventReomved += SubsManager_OnEventRemoved;
    }

    private void SubsManager_OnEventRemoved(object? sender, string e)
    {
        if (_persistentConnetion.IsConnected)
        {
            _persistentConnetion.TryConnect();
        }

        using var channel = _persistentConnetion.CreateModel();
        channel.QueueUnbind(_queueName, _exchangeName, e);
        if (_subsManager.IsEmpty)
        {
            _queueName = string.Empty;
            _consumerChannel.Close();
        }
    }

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnetion.IsConnected)
        {
            _persistentConnetion.TryConnect();
        }

        IModel channel = _persistentConnetion.CreateModel();
        channel.ExchangeDeclare(_exchangeName, "direct");
        channel.QueueDeclare(_queueName, true, false, false, null);

        return channel;
    }

    //Channel 是建立在 Connection 上的虚拟连接
    //创建和销毁 TCP 连接的代价非常高，
    //Connection 可以创建多个 Channel ，Channel 不是线程安全的所以不能在线程间共享。
    public void Publish(string eventName, object? eventData)
    {
        if (_persistentConnetion.IsConnected)
        {
            _persistentConnetion.TryConnect();
        }

        using var channel = _persistentConnetion.CreateModel();
        channel.ExchangeDeclare(_exchangeName, "direct");
        byte[] body;
        if (eventData == null)
        {
            body = new byte[0];
        }
        else
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };
            body = JsonSerializer.SerializeToUtf8Bytes(eventData, eventData.GetType(), options);
        }

        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 2; //persistent

        channel.BasicPublish(_exchangeName,
            eventName,
            true,
            properties,
            body);
    }

    public void Subscribe(string eventName, Type handlerType)
    {
        CheckHanlderType(handlerType);
        DoInternalSubsctiption(eventName);
        _subsManager.AddSubscription(eventName, handlerType);
        StartBasicConsume();
    }

    private void StartBasicConsume()
    {
        if (_consumerChannel != null)
        {
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
            consumer.Received += Consumer_Received;
            _consumerChannel.BasicConsume(_queueName, false, consumer);
        }
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
        try
        {
            await ProcessEvent(eventName, message);
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_subsManager.HasSubscriptionForEvent(eventName))
        {
            List<Type> subscriptions = _subsManager.GetHandlersForEvent(eventName);
            foreach (var subscription in subscriptions)
            {
                using var scope = _serviceProvider.CreateScope();
                var eventHandler = scope.ServiceProvider.GetService(subscription) as IIntegrationEventHandler;
                if (eventHandler == null)
                {
                    throw new ApplicationException($"无法创建{subscription}的服务");
                }

                await eventHandler.Handle(eventName, message);
            }
        }
        else
        {
            var entryAsm = Assembly.GetEntryAssembly().GetName().Name;
            Debug.WriteLine($"找不到可以处理eventName={eventName}的处理程序,entryAsm={entryAsm}");
        }
    }


    private void DoInternalSubsctiption(string eventName)
    {
        var containsEvent = _subsManager.HasSubscriptionForEvent(eventName);
        if (!containsEvent)
        {
            if (!_persistentConnetion.IsConnected)
            {
                _persistentConnetion.TryConnect();
            }

            _consumerChannel.QueueBind(_queueName, _exchangeName, eventName);
        }
    }

    private void CheckHanlderType(Type handlerType)
    {
        if (!typeof(IIntegrationEventHandler).IsAssignableFrom(handlerType))
        {
            throw new ArgumentException($"{handlerType} doesn't inherit from IIntegrationEventHander",
                nameof(handlerType));
        }
    }

    public void UnSubscribe(string eventName, Type handlerType)
    {
        CheckHanlderType(handlerType);
        _subsManager.RemoveSubscription(eventName, handlerType);
    }

    public void Dispose()
    {
        if (_consumerChannel != null)
        {
            _consumerChannel.Dispose();
        }

        _subsManager.Clear();
        _persistentConnetion.Dispose();
        _serviceScope.Dispose();
    }
}