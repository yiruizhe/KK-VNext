using FluentValidation;

namespace Listening.Admin.WebApi.Albums.Request
{
    public record AlbumUpdateRequest(string Name);

    public class AlbumUpdateValidator : AbstractValidator<AlbumUpdateRequest>
    {
        public AlbumUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 200);
        }
    }
}
