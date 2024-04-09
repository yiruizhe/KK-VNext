using Listening.Domain.Events;
using MediatR;

namespace Listening.Admin.WebApi.EventHanders
{
    public class EpisodeCreatedHanlder : INotificationHandler<EpisodeCreateEvent>
    {
        public Task Handle(EpisodeCreateEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
