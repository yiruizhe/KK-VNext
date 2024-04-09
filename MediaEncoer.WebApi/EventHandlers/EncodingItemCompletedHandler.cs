using KK.EventBus;
using MediaEncoder.Domain.Events;
using MediatR;

namespace MediaEncoer.WebApi.EventHandlers
{
    public class EncodingItemCompletedHandler : INotificationHandler<EncodingItemCompletdEvent>
    {
        private readonly IEventBus eventBus;

        public EncodingItemCompletedHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public Task Handle(EncodingItemCompletdEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("MediaEncoding.Completed", notification);
            return Task.CompletedTask;
        }
    }
}
