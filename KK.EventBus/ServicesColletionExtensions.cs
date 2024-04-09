using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Reflection;

namespace KK.EventBus;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName
        , params Assembly[] assemblies)
    {
        return AddEventBus(services, queueName, assemblies.ToList());
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName
        , IEnumerable<Assembly> assemblies)
    {
        List<Type> handlers = new List<Type>();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)));
            handlers.AddRange(types);
        }

        return AddEventBus(services, queueName, handlers);
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName,
        IEnumerable<Type> handlers)
    {
        foreach (var type in handlers)
        {
            services.AddScoped(type, type);
        }

        services.AddSingleton<IEventBus>(sp =>
        {
            var optionMq = sp.GetRequiredService<IOptions<IntegrationEventRabbitMqOptions>>().Value;
            var connectionFactory = new ConnectionFactory()
            {
                HostName = optionMq.HostName,
                DispatchConsumersAsync = true
            };
            var rabbitMqConnection = new RabbitMqConnection(connectionFactory);
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            var eventBus = new RabbitMqEventBus(optionMq.ExchangeName, queueName, rabbitMqConnection,
                scopeFactory);
            foreach (var type in handlers)
            {
                var eventNameAttributes = type.GetCustomAttributes<EventNameAttribute>();
                if (!eventNameAttributes.Any())
                {
                    throw new ApplicationException($"there should be as least one EventNameAttibute on type{type}");
                }

                foreach (var nameAttribute in eventNameAttributes)
                {
                    eventBus.Subscribe(nameAttribute.Name, type);
                }
            }
            return eventBus;
        });
        return services;
    }
}