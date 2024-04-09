using Listening.Domain.Entities;
using MediatR;

namespace Listening.Domain.Events
{
    public record EpisodeUpdateEvent(Episode Value) : INotification;
}
