using FluentValidation;

namespace IdentityService.WebApi.Controllers.Login
{
    public record LoginByUserNameAndPwdRequest(string UserName, string Password);

    public class LoginByUserNameAndPwdValidator : AbstractValidator<LoginByUserNameAndPwdRequest>
    {
        public LoginByUserNameAndPwdValidator()
        {
            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }
}
