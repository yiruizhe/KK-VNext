using KK.EventBus;
using MediaEncoder.Domain.Entities;
using MediaEncoder.Infrustructure;
using Microsoft.EntityFrameworkCore;

namespace MediaEncoer.WebApi.EventHandlers;

[EventName("MediaEncoding.Created")]
public class MediaEncodingCreatedHandler : DynamicIntegrationHanlder
{
    private readonly MEDbcontext ctx;
    private readonly IEventBus eventBus;

    public MediaEncodingCreatedHandler(MEDbcontext ctx, IEventBus eventBus)
    {
        this.ctx = ctx;
        this.eventBus = eventBus;
    }

    public override async Task HandleDynamic(string eventName, dynamic eventData)
    {
        string mediaId = eventData.MediaId;
        Uri sourceUrl = new Uri(eventData.MediaUrl);
        string outputFormat = eventData.OutputFormat;
        string sourceSystem = eventData.SourceSystem;
        string userId = eventData.UserId;
        string fileName = sourceUrl.Segments.Last();
        // 幂等性校验，防止用户重复提交
        bool isExist = await ctx.EncodingItems
            .AnyAsync(e => e.SourceUrl == sourceUrl && e.OutputFormat == outputFormat && e.CreateUser == userId);
        if (isExist)
        {
            return;
        }
        //把任务插入数据库
        var encodingItem = EncodingItem.Create(mediaId, fileName, sourceUrl, outputFormat, sourceSystem, userId);
        ctx.Add(encodingItem);
        await ctx.SaveChangesAsync();
    }
}
