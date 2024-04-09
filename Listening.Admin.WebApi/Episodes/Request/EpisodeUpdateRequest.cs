using FluentValidation;

namespace Listening.Admin.WebApi.Episodes.Request
{
    public record EpisodeUpdateRequest(string Name, string Subtitle, string SubtitleType);

    public class EpisodeUpdateValidator : AbstractValidator<EpisodeUpdateRequest>
    {
        public EpisodeUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Subtitle).NotEmpty();
            RuleFor(x => x.SubtitleType).Length(1, 10);
        }
    }
}
