using Listening.Domain.Entities;

namespace Listening.Main.WebApi.Album
{
    public record AlbumVM(string Id,string Name,string CategoryId)
    {
        public static AlbumVM? Create(Domain.Entities.Album? album)
        {
            if(album == null)
            {
                return null;
            }
            return new AlbumVM(album.Id,album.Name,album.CategoryId);
        }

        public static AlbumVM[] Create(Domain.Entities.Album[] albums)
        {
            return albums.Select(a => Create(a)!).ToArray();
        }
    }
}
