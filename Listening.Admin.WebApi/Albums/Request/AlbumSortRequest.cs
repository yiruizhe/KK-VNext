using FluentValidation;
using Listening.Infrastructure;

namespace Listening.Admin.WebApi.Albums.Request
{
    public record AlbumSortRequest
    {
        public string[] SortedIds { get; set; }

        public class AlbumSortValidator : AbstractValidator<AlbumSortRequest>
        {
            public AlbumSortValidator(ListeningDbContext ctx)
            {
                RuleFor(x => x.SortedIds).NotNull().NotEmpty()
                    .Must(s => !s.Contains(string.Empty))
                    .WithMessage("不能包含空字符")
                    .Must(s => s.Distinct().Count() == s.Count())
                    .WithMessage("不能包含重复Id");
            }
        }
    }
}
