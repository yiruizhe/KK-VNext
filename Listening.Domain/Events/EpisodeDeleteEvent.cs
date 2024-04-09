using MediatR;

namespace Listening.Domain.Events
{
    public record EpisodeDeleteEvent(string id) : INotification;
}
