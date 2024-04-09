using KK.EventBus;
using MediaEncoder.Domain.Events;
using MediatR;

namespace MediaEncoer.WebApi.EventHandlers
{
    public class EncodingItemFailedHanlder : INotificationHandler<EncodingItemFailedEvent>
    {

        private readonly IEventBus eventBus;

        public EncodingItemFailedHanlder(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public Task Handle(EncodingItemFailedEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("MediaEncoding.Failed", notification);
            return Task.CompletedTask;
        }
    }
}
