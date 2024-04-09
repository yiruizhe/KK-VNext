using FluentValidation;

namespace Listening.Admin.WebApi.Categories.Request
{
    public record CategorySortRequest()
    {
        public string[] SortedIds { get; set; }
    }

    public class CategorySortValidator : AbstractValidator<CategorySortRequest>
    {
        public CategorySortValidator()
        {
            RuleFor(x => x.SortedIds).NotNull().NotEmpty()
                .Must(s => !s.Contains(string.Empty))
                .WithMessage("不能包含空字符")
                .Must(s => s.Distinct().Count() == s.Count())
                .WithMessage("不能包含重复Id");
        }
    }
}
