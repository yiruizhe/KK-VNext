using IdentityService.Domain;
using MediatR;

namespace IdentityService.WebApi.Controllers.Events
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly ISmsSender smsSender;

        public UserCreatedEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            smsSender.SendAsync(notification.PhoneNumber, notification.Password);
            return Task.CompletedTask;
        }
    }
}
