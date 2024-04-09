namespace KK.EventBus;

public class SubscritionsManager
{
    private readonly Dictionary<string, List<Type>> _handlers = new();

    public event EventHandler<string> OnEventReomved;

    public bool IsEmpty => _handlers.Any();

    public void Clear() => _handlers.Clear();

    /// <summary>
    /// 把eventHandlerType类型（实现了IIntegrationHandler的类）注册为监听了eventname事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventHandlerType"></param>
    public void AddSubscription(string eventName, Type eventHandlerType)
    {
        if (!HasSubscriptionForEvent(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Contains(eventHandlerType))
        {
            throw new ArgumentException($"Handler type {eventHandlerType} has registed for '{eventName}'",
                nameof(eventHandlerType));
        }

        _handlers[eventName].Add(eventHandlerType);
    }


    public void RemoveSubscription(string eventName, Type handlerType)
    {
        _handlers[eventName].Remove(handlerType);
        if (!_handlers[eventName].Any())
        {
            _handlers.Remove(eventName);
            OnEventReomved?.Invoke(this, eventName);
        }
    }

    /// <summary>
    /// 是否有类型监听了eventname事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public List<Type> GetHandlersForEvent(string eventName) => _handlers[eventName];
}