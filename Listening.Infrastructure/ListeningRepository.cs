using Listening.Domain;
using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listening.Infrastructure;

public class ListeningRepository : IListeningRepository
{
    private readonly ListeningDbContext ctx;

    public ListeningRepository(ListeningDbContext ctx)
    {
        this.ctx = ctx;
    }

    public Task<Album?> FindAlbumByIdAsync(string id)
    {
        return ctx.Albums.FirstOrDefaultAsync(a => a.Id.Equals(id));
    }

    public Task<Category?> FindCategoryByIdAsync(string id)
    {
        return ctx.Categories.FirstOrDefaultAsync(a => a.Id.Equals(id));
    }

    public Task<Episode?> FindEpisodeByIdAsync(string id)
    {
        return ctx.Episodes.FirstOrDefaultAsync(a => a.Id.Equals(id));
    }

    public Task<Album[]> GetAlbumsByCategoryIdAsync(string categoryId)
    {
        return ctx.Albums.Where(a => a.CategoryId.Equals(categoryId)).OrderBy(a => a.SequenceNumber).ToArrayAsync();
    }

    public Task<Category[]> GetCategoriesAsync()
    {
        return ctx.Categories.OrderBy(c => c.SequenceNumber).ToArrayAsync();
    }

    public Task<Episode[]> GetEpisodesByAlbumIdAsync(string albumId)
    {
        return ctx.Episodes.Where(a => a.AlbumId.Equals(albumId)).OrderBy(e => e.SequenceNumber).ToArrayAsync();
    }

    public async Task<int> GetMaxSeqOfAlbumsAsync(string categoryId)
    {
        int? maxSeq = await ctx.Albums.Where(a => a.CategoryId.Equals(categoryId)).MaxAsync(a => (int?)a.SequenceNumber);
        return maxSeq ?? 0;
    }

    public async Task<int> GetMaxSeqOfCategoriesAsync()
    {
        int? maxSeq = await ctx.Categories.MaxAsync(c => (int?)c.SequenceNumber);
        return maxSeq ?? 0;
    }

    public async Task<int> GetMaxSeqOfEpisodesAsync(string albumId)
    {
        int? maxSeq = await ctx.Episodes.Where(a => a.AlbumId.Equals(albumId)).MaxAsync(a => (int?)a.SequenceNumber);
        return maxSeq ?? 0;
    }
}
