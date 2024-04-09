using FileService.Domain;
using FileService.Infrasructure;
using KK.ASPNETCORE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileService.WebApi.Uploader;

[Route("[controller]/[action]")]
[ApiController]
[Authorize(Roles = "Admin")]
[UnitOfWork(typeof(FSDbContext))]
public class UploadController : ControllerBase
{
    private readonly FSDbContext ctx;
    private readonly IFSRepository repository;
    private readonly FSDomainService domainService;
    private readonly ILogger<UploadController> logger;

    public UploadController(FSDbContext ctx, IFSRepository repository, FSDomainService domainService,
        ILogger<UploadController> logger)
    {
        this.ctx = ctx;
        this.repository = repository;
        this.domainService = domainService;
        this.logger = logger;
    }

    /// <summary>
    /// 检查是否有大小以及sha256hash值相同的文件
    /// </summary>
    [HttpGet]
    public async Task<FileExistResponse> FileExist(long fileSize, string sha256Hash)
    {
        var item = await repository.FindAsync(fileSize, sha256Hash);
        if (item != null)
        {
            return new FileExistResponse(true, item.RemoteUrl);
        }
        else
        {
            return new FileExistResponse(false, null);
        }
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(60_000_000)]
    [Validate(typeof(FileUploadValidator))]
    public async Task<ActionResult<Uri>> Upload([FromForm] FileUploadRequest req, CancellationToken cancellationToken = default)
    {
        var file = req.File;
        string fileName = file.FileName;
        var stream = file.OpenReadStream();
        var uploadItem = await domainService.UploadAsync(stream, fileName, cancellationToken);
        var item = await repository.FindAsync(uploadItem.FileSizeInBytes, uploadItem.FileSHA256Hash);
        if (item != null)
        {
            return BadRequest("请勿重复上传相同文件");
        }
        ctx.UploadedItems.Add(uploadItem);
        return Ok(uploadItem.RemoteUrl);
    }
}
