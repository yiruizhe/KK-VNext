using IdentityService.Domain;
using MediatR;

namespace IdentityService.WebApi.Controllers.Events
{
    public class ResetPasswordEventHandler : INotificationHandler<ResetPasswordEvent>
    {
        private readonly ISmsSender smsSender;

        public ResetPasswordEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        public Task Handle(ResetPasswordEvent notification, CancellationToken cancellationToken)
        {
            smsSender.SendAsync(notification.PhoneNumber, notification.Password);
            return Task.CompletedTask;
        }
    }
}
