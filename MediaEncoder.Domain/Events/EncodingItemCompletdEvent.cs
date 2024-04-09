using MediatR;

namespace MediaEncoder.Domain.Events
{
    public record EncodingItemCompletdEvent(string Id, string SourceSystem, Uri OutputUrl, string UserId)
        : INotification;
}
