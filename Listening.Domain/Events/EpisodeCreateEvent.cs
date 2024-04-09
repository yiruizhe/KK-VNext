

using Listening.Domain.Entities;
using MediatR;

namespace Listening.Domain.Events
{
    public record EpisodeCreateEvent(Episode Value) : INotification;
}
