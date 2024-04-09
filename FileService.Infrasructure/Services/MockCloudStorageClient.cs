using FileService.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FileService.Infrasructure.Services;

public class MockCloudStorageClient : IStorageClient
{
    public StorageType StorageType => StorageType.Public;

    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;

    public MockCloudStorageClient(IWebHostEnvironment webHostEnvironment,
         IHttpContextAccessor httpContextAccessor)
    {
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
    {
        if (key.StartsWith("/"))
        {
            throw new ArgumentException("");
        }
        string workDir = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot");
        string fullPath = Path.Combine(workDir, key);
        string? fullDir = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullDir);
        }
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        using var fs = File.OpenWrite(fullPath);
        await content.CopyToAsync(fs, cancellationToken);
        await fs.FlushAsync();
        var req = httpContextAccessor.HttpContext.Request;
        string url = req.Scheme + "://" + req.Host + "/" + "FileService/" + key;
        return new Uri(url);
    }
}
