using FileService.Domain;
using Microsoft.Extensions.Options;

namespace FileService.Infrasructure.Services;

public class SMBStorageClient : IStorageClient
{
    private readonly IOptions<SMBStorageOptions> options;

    public SMBStorageClient(IOptions<SMBStorageOptions> options)
    {
        this.options = options;
    }

    public StorageType StorageType => StorageType.Backup;

    /// <summary>
    /// 保存至备份
    /// </summary>
    /// <param name="key"></param>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
    {
        if (key.StartsWith("/"))
        {
            throw new ArgumentException("key should not start with /", nameof(key));
        }
        string rootDir = options.Value.WorkDir;
        string fullPath = Path.Combine(rootDir, key);
        string? fullDir = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(fullDir))
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
        return new Uri(fullPath);
    }
}
