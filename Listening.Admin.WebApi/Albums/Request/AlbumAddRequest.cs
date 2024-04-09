using FluentValidation;
using Listening.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Listening.Admin.WebApi.Albums.Request
{
    public record AlbumAddRequest(string Name, string CategoryId);

    public class AlbumAddValidator : AbstractValidator<AlbumAddRequest>
    {
        public AlbumAddValidator(ListeningDbContext ctx)
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 200);
            RuleFor(x => x.CategoryId).NotEmpty()
                .MustAsync((x, _) => ctx.Categories.AnyAsync(c => c.Id.Equals(x)))
                .WithMessage(x => $"CategoryId={x.CategoryId}不存在");
        }
    }
}
