using KK.EventBus;
using MediaEncoder.Domain.Events;
using MediatR;

namespace MediaEncoer.WebApi.EventHandlers
{
    public class EncodingItemStartedHanlder : INotificationHandler<EncodingItemStartedEvent>
    {
        private readonly IEventBus eventBus;

        public EncodingItemStartedHanlder(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public Task Handle(EncodingItemStartedEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("MediaEncoding.Started", notification);
            return Task.CompletedTask;
        }
    }
}
