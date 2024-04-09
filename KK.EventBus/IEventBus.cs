namespace KK.EventBus;

public interface IEventBus
{
    void Publish(string eventName, object? eventData);

    void Subscribe(string eventName, Type handlerType);

    void UnSubscribe(string eventName, Type handlerType);
}