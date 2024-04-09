using FluentValidation;

namespace Listening.Admin.WebApi.Categories.Request
{
    public record CategoryUpdateRequest(string Name, Uri CoverUrl);

    public class CategoryUpdateValidator : AbstractValidator<CategoryUpdateRequest>
    {
        public CategoryUpdateValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().Length(1, 200);
        }
    }
}
