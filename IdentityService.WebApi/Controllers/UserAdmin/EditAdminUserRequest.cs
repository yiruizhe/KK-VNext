using FluentValidation;

namespace IdentityService.WebApi.Controllers.UserAdmin
{
    public record EditAdminUserRequest(string PhoneNum);

    public class EditAdminUserValidator : AbstractValidator<EditAdminUserRequest>
    {
        public EditAdminUserValidator()
        {
            RuleFor(x => x.PhoneNum).NotEmpty().MaximumLength(11);
        }
    }
}
