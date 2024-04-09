using MediatR;

namespace MediaEncoder.Domain.Events
{
    public record EncodingItemStartedEvent(string Id, string SourceSystem, string UserId)
        : INotification
    {
    }
}
