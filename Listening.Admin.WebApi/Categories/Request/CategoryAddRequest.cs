using FluentValidation;

namespace Listening.Admin.WebApi.Categories.Request
{
    public record CategoryAddRequest(string Name, Uri CoverUrl);

    public class CategoryAddValidator : AbstractValidator<CategoryAddRequest>
    {
        public CategoryAddValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().Length(1, 200);
        }
    }
}
