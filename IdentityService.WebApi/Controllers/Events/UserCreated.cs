using MediatR;

namespace IdentityService.WebApi.Controllers.Events
{
    public record UserCreatedEvent(string Id, string UserName, string Password, string PhoneNumber) : INotification;
    public record ResetPasswordEvent(string Id, string UserName, string Password, string PhoneNumber) : INotification;
}
