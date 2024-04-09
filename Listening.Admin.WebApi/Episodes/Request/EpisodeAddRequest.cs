using FluentValidation;
using Listening.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Listening.Admin.WebApi.Episodes.Request
{
    public record EpisodeAddRequest(string Name, string AlbumId, Uri AudioUrl,
        double DurationInSecond, string Subtitle, string SubtitleType);

    public class EpisodeAddValidator : AbstractValidator<EpisodeAddRequest>
    {
        public EpisodeAddValidator(ListeningDbContext ctx)
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.AlbumId).MustAsync((x, _) => ctx.Albums.AnyAsync(a => a.Id.Equals(x)))
                .WithMessage(x => $"AlbumId={x.AlbumId}不存在");
            RuleFor(x => x.AudioUrl).NotEmpty().Must(x => x.OriginalString.Length > 0 && x.OriginalString.Length < 1000)
                .WithMessage($"Url长度必须在0到1000之间");
            RuleFor(x => x.DurationInSecond).Must(x => x > 0);
            RuleFor(x => x.Subtitle).NotEmpty();
            RuleFor(x => x.SubtitleType).Length(1, 10);
        }
    }
}
