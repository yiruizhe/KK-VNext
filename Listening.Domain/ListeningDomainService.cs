using KK.Commons;
using Listening.Domain.Entities;
using static Listening.Domain.Entities.Episode;

namespace Listening.Domain;

public class ListeningDomainService
{
    private readonly IListeningRepository repository;

    public ListeningDomainService(IListeningRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Category> AddCategoryAsync(string name, Uri coverUrl)
    {
        int maxSeq = await repository.GetMaxSeqOfCategoriesAsync();
        var category = Category.Create(name, coverUrl, maxSeq + 1);
        return category;
    }

    public async Task<Album> AddAlbumAsync(string name, string categoryId)
    {
        int maxSeq = await repository.GetMaxSeqOfAlbumsAsync(categoryId);
        return Album.Create(maxSeq + 1, name, categoryId);
    }

    public async Task<Episode> AddEpisodeAsync(string name, string subtitle, string subtitleType,
        string albumId, Uri audioUrl, double durationInSecond)
    {
        int maxSeq = await repository.GetMaxSeqOfEpisodesAsync(albumId);
        return new EpisodeBuilder().Name(name).SequenceNumber(maxSeq + 1).Subtitle(subtitle)
            .SubtitleType(subtitleType).AlbumId(albumId).AudioUrl(audioUrl).DurationInSecond(durationInSecond)
            .Build();
    }

    /// <summary>
    /// 根据id顺序调整序列号
    /// </summary>
    /// <param name="sortedCategoryIds"></param>
    /// <returns></returns>
    public async Task SortCategoriesAsync(string[] sortedCategoryIds)
    {

        Category[] categories = await repository.GetCategoriesAsync();
        var dbIds = categories.Select(c => c.Id);
        if (!dbIds.SequenceIgnoreEquals(sortedCategoryIds))
        {
            throw new ArgumentException("提交的id必须包含全部分类id");
        }
        int seqNum = 1;
        foreach (var id in sortedCategoryIds)
        {
            Category? category = await repository.FindCategoryByIdAsync(id);
            if (category == null)
            {
                throw new ArgumentException($"id={id}不存在");
            }
            category.ChangeSequenceNumber(seqNum);
            seqNum++;
        }
    }

    /// <summary>
    /// 根据提交的id调整专辑序列
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="sortedAlbumIds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task SortAlbumsAsync(string categoryId, string[] sortedAlbumIds)
    {
        Album[] albums = await repository.GetAlbumsByCategoryIdAsync(categoryId);
        var dbIds = albums.Select(c => c.Id);
        if (!dbIds.SequenceIgnoreEquals(sortedAlbumIds))
        {
            throw new ArgumentException("提交的id必须包含全部专辑id");
        }
        int seqNum = 1;
        foreach (var id in sortedAlbumIds)
        {
            Album? album = await repository.FindAlbumByIdAsync(id);
            if (album == null)
            {
                throw new ArgumentException($"id={id}不存在");
            }
            album.ChangeSequenceNumber(seqNum);
            seqNum++;
        }
    }

    public async Task SortEpisodesAsync(string albumId, string[] sortedEpisodeIds)
    {
        Episode[] episodes = await repository.GetEpisodesByAlbumIdAsync(albumId);
        var dbIds = episodes.Select(c => c.Id);
        if (!dbIds.SequenceIgnoreEquals(sortedEpisodeIds))
        {
            throw new ArgumentException("提交的id必须包含全部音频id");
        }
        int seqNum = 1;
        foreach (var id in sortedEpisodeIds)
        {
            Episode? episode = await repository.FindEpisodeByIdAsync(id);
            if (episode == null)
            {
                throw new ArgumentException($"id={id}不存在");
            }
            episode.ChangeSequenceNumber(seqNum);
            seqNum++;
        }
    }


}
