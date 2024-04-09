using KK.ASPNETCORE;
using KK.Commons;
using KK.EventBus;
using Listening.Admin.WebApi.Episodes.Request;
using Listening.Domain;
using Listening.Domain.Entities;
using Listening.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Listening.Admin.WebApi.Episodes;

[Authorize(Roles = "Admin")]
[UnitOfWork(typeof(ListeningDbContext))]
[Route("[controller]/[action]")]
[ApiController]
public class EpisodeController : ControllerBase
{
    private readonly IListeningRepository repository;
    private readonly ListeningDbContext context;
    private readonly ListeningDomainService domainService;
    private readonly IEventBus eventBus;
    private readonly EncodingEpisodeCacheHelper episodeHelper;

    public EpisodeController(IListeningRepository repository, ListeningDbContext context,
        ListeningDomainService domainService, IEventBus eventBus, EncodingEpisodeCacheHelper encodingEpisodeHelper)
    {
        this.repository = repository;
        this.context = context;
        this.domainService = domainService;
        this.eventBus = eventBus;
        this.episodeHelper = encodingEpisodeHelper;
    }

    [HttpPost]
    //实现校验请求数据的过滤器
    [Validate(typeof(EpisodeAddValidator))]
    public async Task<ActionResult<string>> Add(EpisodeAddRequest req)
    {
        //如果是m4a文件则不需要转码
        if (req.AudioUrl.ToString().EndsWith("m4a", StringComparison.OrdinalIgnoreCase))
        {
            var episode = await domainService.AddEpisodeAsync(req.Name, req.Subtitle, req.SubtitleType,
                req.AlbumId, req.AudioUrl, req.DurationInSecond);
            context.Episodes.Add(episode);
            return episode.Id;
        }
        else
        {
            // 非m4a文件需转码再插入数据库，先临时写入redis，转码完成后再插入数据库
            string id = IdHelper.NextId;
            var encodingEpisode = new EncodingEpisodeInfo(id, req.Name, req.Subtitle, req.SubtitleType,
                req.AlbumId, req.DurationInSecond, "Created");
            // 写入redis
            await episodeHelper.AddEncodingEpisodeAsync(id, encodingEpisode);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // 发布转码
            eventBus.Publish("MediaEncoding.Created",
                new { MediaId = id, MediaUrl = req.AudioUrl, OutputFormat = "m4a", SourceSystem = "Listening", UserId = userId });
            return id;
        }
    }

    [HttpPut("{id}")]
    [Validate(typeof(EpisodeUpdateValidator))]
    public async Task<ActionResult> Update(string id, EpisodeUpdateRequest req)
    {
        var episode = await repository.FindEpisodeByIdAsync(id);
        if (episode == null)
        {
            return NotFound("id不存在");
        }
        episode.ChangeName(req.Name);
        episode.ChangeSubTitle(req.SubtitleType, episode.SubTitle);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var episode = await repository.FindEpisodeByIdAsync(id);
        if (episode == null)
            return NotFound("id不存在");
        episode.SoftDelete();
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Episode>> FindById(string id)
    {
        var episode = await repository.FindEpisodeByIdAsync(id);
        if (episode == null)
            return NotFound("id不存在");
        return episode;
    }

    [HttpGet("{albumId}")]
    public async Task<ActionResult<Episode[]>> FindByAlbumId(string albumId)
    {
        return await repository.GetEpisodesByAlbumIdAsync(albumId);
    }


    /// <summary>
    /// 查询该专辑下所有待转码的音频
    /// </summary>
    /// <param name="albumId"></param>
    /// <returns></returns>
    [HttpGet("{albumId}")]
    public async Task<ActionResult<EncodingEpisodeInfo[]>> FindEncodingEpisodesByAlbumId(string albumId)
    {
        IEnumerable<string> ids
            = await episodeHelper.GetEncodingEpisodesIdsAsync(albumId);
        var encodingEpisodeInfos = new List<EncodingEpisodeInfo>();
        foreach (var id in ids)
        {
            var encodingEpisode = await episodeHelper.GetEncodingEpisodeAsync(id);
            if (!encodingEpisode.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                encodingEpisodeInfos.Add(encodingEpisode);
            }
        }
        return encodingEpisodeInfos.ToArray();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Hide(string id)
    {
        var episode = await repository.FindEpisodeByIdAsync(id);
        if (episode == null)
            return NotFound("id不存在");
        episode.Hide();
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Show(string id)
    {
        var episode = await repository.FindEpisodeByIdAsync(id);
        if (episode == null)
            return NotFound("id不存在");
        episode.Show();
        return Ok();
    }

    [HttpPut("{albumId}")]
    [Validate(typeof(EpisodeSortValidator))]
    public async Task<ActionResult> Sort(string albumId, EpisodeSortRequest req)
    {
        await domainService.SortEpisodesAsync(albumId, req.SortedEpisodeIds);
        return Ok();
    }

}
