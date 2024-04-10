using KK.ASPNETCORE;
using Listening.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Main.WebApi.Episode;

[Route("[controller]/[action]")]
[ApiController]
public class EpisodeController : ControllerBase
{
    private readonly IDistributeCacheHelper cacheHelper;
    private readonly IListeningRepository repository;

    public EpisodeController(IDistributeCacheHelper cacheHelper, IListeningRepository repository)
    {
        this.cacheHelper = cacheHelper;
        this.repository = repository;
    }

    [HttpGet("{episodeId}")]
    public async Task<ActionResult<EpisodeVM?>> FindOne(string episodeId)
    {
        var result = await cacheHelper.GetOrCreateAsync($"Episode_{episodeId}", async opt =>
        {
            var episode = await repository.FindEpisodeByIdAsync(episodeId);
            if (episode == null)
            {
                return null;
            }
            return EpisodeVM.Create(episode.IsVisible ? episode : null, true);
        });
        if (result == null)
        {
            return NotFound($"找不到Id={episodeId}的音频信息");
        }
        return result;
    }

    [HttpGet("{albumId}")]
    public async Task<ActionResult<EpisodeVM[]>> FindAllByAlbumId(string albumId)
    {
        var result = await cacheHelper.GetOrCreateAsync($"Episode_All_{albumId}", async opt =>
        {
            var result = (await repository.GetEpisodesByAlbumIdAsync(albumId)).Where(e => e.IsVisible).ToArray();
            return EpisodeVM.Create(result);
        });
        return result;
    }
}
