using FluentValidation;

namespace IdentityService.WebApi.Controllers.Login
{
    public record LoginByPhoneAndPwdRequest(string PhoneNumber, string Password);

    public class LoginByPhoneAndPwdValidator : AbstractValidator<LoginByPhoneAndPwdRequest>
    {
        public LoginByPhoneAndPwdValidator()
        {
            RuleFor(x => x.PhoneNumber).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }

}
