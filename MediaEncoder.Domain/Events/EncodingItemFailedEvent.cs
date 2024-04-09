using MediatR;

namespace MediaEncoder.Domain.Events
{
    public record EncodingItemFailedEvent(string Id, string SourceSystem, string ErrorMsg, string UserId)
        : INotification;
}
