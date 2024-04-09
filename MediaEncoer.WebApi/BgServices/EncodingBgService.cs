using FileService.SDK.NETCore;
using KK.ASPNETCORE;
using KK.Commons;
using KK.EventBus;
using KK.JWT;
using MediaEncoder.Domain;
using MediaEncoder.Domain.Entities;
using MediaEncoder.Infrustructure;
using MediaEncoer.WebApi.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace MediaEncoer.WebApi.BgServices;


public class EncodingBgService : BackgroundService//singleton
{
    private readonly IServiceScope serviceScope;
    private readonly IOptions<FileServiceOptions> urlRootOptions;
    private readonly IMediaEncoderRepository repository;
    private readonly MEDbcontext ctx;
    private readonly IConnectionMultiplexer connectionMultiplexer;//singleton
    private readonly ILogger<EncodingBgService> logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IEventBus eventBus;
    private readonly MediaEncoderFactory encoderFactory;
    private readonly ITokenService tokenService;
    private readonly IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot;

    public EncodingBgService(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScope = serviceScopeFactory.CreateScope();
        var sp = serviceScope.ServiceProvider;
        // TODO 注入服务
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // 查询所有状态ready的emcodingitem
            var encodingItems = await repository.FindAsync(ItemStatus.Ready);
            /*
            // 将全部转码任务并行处理
             List<Task> tasks = new();
             foreach (var item in encodingItems)
             {
                 tasks.Add(ProcessItemAsync(item, stoppingToken));
             }
             await Task.WhenAll(tasks);
            */
            // 因为转码比较消耗cpu等资源，所有串行转码
            foreach (var item in encodingItems)
            {
                try
                {
                    await ProcessItemAsync(item, stoppingToken);
                }
                catch (Exception e)
                {
                    item.Fail(e);
                }
                await ctx.SaveChangesAsync(stoppingToken);
            }
            await Task.Delay(5000);// 暂停5s，避免没有任务时CPU空转
        }
    }

    /// <summary>
    /// 处理该待转码文件
    /// </summary>
    /// <param name="readyItem"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task ProcessItemAsync(EncodingItem readyItem, CancellationToken ct)
    {
        string mediaId = readyItem.Id;
        TimeSpan expireTime = TimeSpan.FromSeconds(30);
        string lockKey = $"MediaEncoder.EncodingItem.{mediaId}";
        var distributedLock = new RedisDistributedLock(connectionMultiplexer.GetDatabase());
        if (!await distributedLock.AcquireLockAsync(lockKey, expireTime))
        {
            // 获取锁失败，可能任务已经被别的媒体转码服务拿走
            logger.LogInformation($"获取lockKey={lockKey}失败，任务已被其他服务处理");
            return;// 处理下一个
        }
        // 拿到锁
        readyItem.Start();// 通知该任务已开始处理
        await ctx.SaveChangesAsync();
        // 下载原文件
        (bool isOk, FileInfo sourceFileInfo) = await DownloadAsync(readyItem, ct);
        if (!isOk)
        {
            readyItem.Fail("下载原文件失败");
            return;
        }
        FileInfo destFileInfo = BuildDestFileInfo(readyItem);
        try
        {
            logger.LogInformation($"下载Id={mediaId}成功，正在计算Hash值");
            long fileSize = sourceFileInfo.Length;
            string fileHash = ComputeHash(sourceFileInfo);
            var prevInstance = await repository.FindCompletedOneAsync(fileSize, fileHash);
            if (prevInstance != null)
            {
                // 已存在相同文件
                logger.LogInformation($"检查Id={mediaId}Hash值计算完成,存在相同大小和Hash值的文件");
                eventBus.Publish("MediaEncoding.Duplicated", new { readyItem.Id, readyItem.SourceSystem, prevInstance.OutputUrl, UserId = readyItem.CreateUser });
                readyItem.Complete(prevInstance.OutputUrl);
                return;
            }
            // 开始转码
            logger.LogInformation($"Id={mediaId}开始转码,源路径:{sourceFileInfo},目标路径:{destFileInfo}");
            bool res = await EncodeAsync(sourceFileInfo, destFileInfo, readyItem.OutputFormat, ct);
            if (!res)
            {
                readyItem.Fail("转码失败");
                return;
            }
            //上传至文件服务器
            logger.LogInformation($"Id={mediaId}转码成功，开始准备上传");
            Uri destUrl = await UploadAsync(destFileInfo, ct);
            readyItem.Complete(destUrl);
            readyItem.ChangeFileMetaData(fileSize, fileHash);
            logger.LogInformation($"Id={mediaId}转码结果上传成功");
        }
        finally
        {
            //释放锁
            distributedLock.ReleaseLock(lockKey);
            //清理缓存
            sourceFileInfo.Delete();
            destFileInfo.Delete();
        }
    }

    private Task<Uri> UploadAsync(FileInfo destFileInfo, CancellationToken ct)
    {
        Uri serverRootUrl = urlRootOptions.Value.UrlRoot;
        var fileServiceClient =
            new FileServiceClient(serverRootUrl, httpClientFactory, tokenService, jwtOptionsSnapshot.Value);
        return fileServiceClient.UploadAsync(destFileInfo, ct);
    }

    private async Task<bool> EncodeAsync(FileInfo sourceFile, FileInfo destFile, string outputFormat, CancellationToken ct)
    {
        IMediaEncoder? mediaEncoder = encoderFactory.Crete(outputFormat);
        if (mediaEncoder == null)
        {
            logger.LogError($"转码失败，找不到转码器，格式:{outputFormat}");
            return false;
        }
        try
        {
            await mediaEncoder.EncodeAsync(sourceFile, destFile, outputFormat, null, ct);
        }
        catch (Exception e)
        {
            logger.LogError("转码失败", e);
            return false;
        }
        return true;
    }

    private static FileInfo BuildDestFileInfo(EncodingItem encodingItem)
    {
        string outputFormat = encodingItem.OutputFormat;
        string tempDir = Path.GetTempPath();
        string destFileFullPath = Path.Combine(tempDir, Guid.NewGuid() + "." + outputFormat);
        return new FileInfo(destFileFullPath);
    }

    private static string ComputeHash(FileInfo fileInfo)
    {
        using var fs = fileInfo.OpenRead();
        return HashHelper.ComputeSha256Hash(fs);
    }

    private async Task<(bool isOk, FileInfo srcFileInfo)> DownloadAsync(EncodingItem encodingItem, CancellationToken ct)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "MediaEncodingDir");
        string sourceFileFullPath = Path.Combine(tempDir, Guid.NewGuid() + "." + Path.GetExtension(encodingItem.FileName));
        var sourceFileInfo = new FileInfo(sourceFileFullPath);
        sourceFileInfo.Directory!.Create();

        var httpClient = httpClientFactory.CreateClient();
        logger.LogInformation($"Id={encodingItem.Id},准备从{encodingItem.SourceUrl}下载到{sourceFileFullPath}");
        var httpResponse = await httpClient.GetAsync(encodingItem.SourceUrl, ct);
        if (!httpResponse.IsSuccessStatusCode)
        {
            logger.LogWarning($"下载Id={encodingItem.Id},Url={encodingItem.SourceUrl}失败,StatusCode={httpResponse.StatusCode}");
            sourceFileInfo.Delete();
            return (false, sourceFileInfo);
        }
        else
        {
            using var fs = new FileStream(sourceFileFullPath, FileMode.Create);
            await httpResponse.Content.CopyToAsync(fs);
            return (true, sourceFileInfo);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        this.serviceScope.Dispose();
    }
}
