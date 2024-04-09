using System.Text.Json;

namespace KK.EventBus;

public abstract class JsonIntegrationEventHandler<T> : IIntegrationEventHandler
{
    public Task Handle(string eventName, string eventData)
    {
        T? deserialize = JsonSerializer.Deserialize<T>(eventData);
        return HandleJson(eventName, deserialize);
    }

    public abstract Task HandleJson(string eventName, T? eventData);
}