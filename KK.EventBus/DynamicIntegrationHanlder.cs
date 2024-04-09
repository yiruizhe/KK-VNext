using Dynamic.Json;

namespace KK.EventBus
{
    public abstract class DynamicIntegrationHanlder : IIntegrationEventHandler
    {
        public Task Handle(string eventName, string eventData)
        {
            dynamic dynamicData = DJson.Parse(eventData);
            return HandleDynamic(eventName, dynamicData);
        }

        public abstract Task HandleDynamic(string eventName, dynamic eventData);

    }
}
