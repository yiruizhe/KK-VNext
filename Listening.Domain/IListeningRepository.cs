

using Listening.Domain.Entities;

namespace Listening.Domain
{
    public interface IListeningRepository
    {
        Task<Album?> FindAlbumByIdAsync(string id);
        Task<Category?> FindCategoryByIdAsync(string id);
        Task<Episode?> FindEpisodeByIdAsync(string id);
        Task<Album[]> GetAlbumsByCategoryIdAsync(string categoryId);
        Task<Category[]> GetCategoriesAsync();
        Task<Episode[]> GetEpisodesByAlbumIdAsync(string albumId);
        Task<int> GetMaxSeqOfAlbumsAsync(string categoryId);
        Task<int> GetMaxSeqOfCategoriesAsync();
        Task<int> GetMaxSeqOfEpisodesAsync(string albumId);
    }
}
