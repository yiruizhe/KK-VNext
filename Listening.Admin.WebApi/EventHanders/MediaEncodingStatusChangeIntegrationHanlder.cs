using KK.EventBus;
using Listening.Admin.WebApi.Episodes;
using Listening.Admin.WebApi.Hubs;
using Listening.Domain;
using Listening.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace Listening.Admin.WebApi.EventHanders;

[EventName("MediaEncoding.Started")]
[EventName("MediaEncoding.Failed")]
[EventName("MediaEncoding.Duplicated")]
[EventName("MediaEncoding.Completed")]
public class MediaEncodingStatusChangeIntegrationHanlder : DynamicIntegrationHanlder
{
    private readonly IHubContext<EpisodeEncodingStatusHub> hubContext;
    private readonly EncodingEpisodeCacheHelper cacheHelper;
    private readonly ListeningDbContext ctx;
    private readonly ListeningDomainService domainService;

    public MediaEncodingStatusChangeIntegrationHanlder(IHubContext<EpisodeEncodingStatusHub> hubContext,
        EncodingEpisodeCacheHelper cacheHelper, ListeningDbContext ctx, ListeningDomainService domainService)
    {
        this.hubContext = hubContext;
        this.cacheHelper = cacheHelper;
        this.ctx = ctx;
        this.domainService = domainService;
    }

    public override async Task HandleDynamic(string eventName, dynamic eventData)
    {
        string sourceSystem = eventData.SourceSystem;

        if (sourceSystem != "Listening")
            return;

        string userId = eventData.UserId;
        string episodeId = eventData.Id;
        Uri outputUrl = eventData.OutputUrl;
        switch (eventName)
        {
            case "MediaEncoding.Started":
                await cacheHelper.UpdateEncodingEpisodeStatusAsync(episodeId, "Started");
                await hubContext.Clients.User(userId).SendAsync("OnMediaEncodingStarted", episodeId);
                break;
            case "MediaEncoding.Failed":
                await cacheHelper.UpdateEncodingEpisodeStatusAsync(episodeId, "Failed");
                await hubContext.Clients.User(userId).SendAsync("OnMediaEncodingFailed", episodeId);
                break;
            case "MediaEncoding.Duplicated":
                await cacheHelper.UpdateEncodingEpisodeStatusAsync(episodeId, "Completed");
                await hubContext.Clients.User(userId).SendAsync("OnMediaEncodingCompleted", episodeId);
                break;
            case "MediaEncoding.Completed":
                await cacheHelper.UpdateEncodingEpisodeStatusAsync(episodeId, "Completed");
                EncodingEpisodeInfo episodeInfo = await cacheHelper.GetEncodingEpisodeAsync(episodeId);
                var episode = await domainService.AddEpisodeAsync(episodeInfo.Name, episodeInfo.Subtitle, episodeInfo.SubtitleType,
                     episodeInfo.AlbumId, outputUrl, episodeInfo.DurationInSecond);
                ctx.Episodes.Add(episode);
                await ctx.SaveChangesAsync();
                await hubContext.Clients.User(userId).SendAsync("OnMediaEncodingCompleted", episodeId);
                // 清理缓存中的数据
                await cacheHelper.RemoveEncodingEpisodeAsync(episodeId, episodeInfo.AlbumId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventName));
        }
    }
}
