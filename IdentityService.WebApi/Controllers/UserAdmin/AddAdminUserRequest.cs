using FluentValidation;

namespace IdentityService.WebApi.Controllers.UserAdmin
{
    public record AddAdminUserRequest(string UserName, string PhoneNumber);

    public class AddAdminUserValidator : AbstractValidator<AddAdminUserRequest>
    {
        public AddAdminUserValidator()
        {
            RuleFor(x => x.UserName).NotNull().NotEmpty().MaximumLength(20).MinimumLength(2);
            RuleFor(x => x.PhoneNumber).NotNull().NotEmpty().MaximumLength(11);
        }
    }
}
