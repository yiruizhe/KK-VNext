using FluentValidation;

namespace IdentityService.WebApi.Controllers.Login
{
    public record ChangeMyPwdRequest(string OldPassword, string NewPassword, string NewPassword2);

    public class ChangeMyPwdValidator : AbstractValidator<ChangeMyPwdRequest>
    {
        public ChangeMyPwdValidator()
        {
            RuleFor(x => x.OldPassword).NotNull().NotEmpty();
            RuleFor(x => x.NewPassword).NotNull().NotEmpty().MinimumLength(6).WithMessage("密码长度不能低于6");
            RuleFor(x => x.NewPassword2).NotNull().NotEmpty().Equal(x => x.NewPassword).WithMessage("密码不一致");
        }
    }

}
