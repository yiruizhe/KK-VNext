using FluentValidation;

namespace Listening.Admin.WebApi.Episodes.Request
{
    public record EpisodeSortRequest
    {
        public string[] SortedEpisodeIds { get; set; }
    }

    public class EpisodeSortValidator : AbstractValidator<EpisodeSortRequest>
    {
        public EpisodeSortValidator()
        {
            RuleFor(x => x.SortedEpisodeIds).NotEmpty()
                .Must(x => x.Distinct().Count() == x.Count())
                .WithMessage("包含重复id")
                .Must(x => !x.Contains(string.Empty))
                .WithMessage("不能包含空字符");
        }
    }
}
