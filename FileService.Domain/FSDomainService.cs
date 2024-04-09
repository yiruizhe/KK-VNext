using FileService.Domain.Entities;
using KK.Commons;

namespace FileService.Domain;

public class FSDomainService
{
    private readonly IFSRepository repository;
    private readonly IStorageClient backupStorage;
    private readonly IStorageClient publicStorage;

    public FSDomainService(IFSRepository repository, IEnumerable<IStorageClient> storageClients)
    {
        this.repository = repository;
        backupStorage = storageClients.First(s => s.StorageType == StorageType.Backup);
        publicStorage = storageClients.First(s => s.StorageType == StorageType.Public);
    }

    /// <summary>
    /// 上传文件，如果已存在直接返回已存在的文件item
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UploadedItem> UploadAsync(Stream stream, string fileName, CancellationToken cancellationToken)
    {
        string hash = HashHelper.ComputeSha256Hash(stream);
        long fileSize = stream.Length;
        DateTime today = DateTime.Today;
        string key = $"{today.Year}/{today.Month}/{today.Day}/{hash}/{fileName}";

        var oldUploadedItem = await repository.FindAsync(fileSize, hash);
        if (oldUploadedItem != null)
        {
            return oldUploadedItem;
        }
        Uri backupUrl = await backupStorage.SaveAsync(key, stream, cancellationToken);
        stream.Position = 0;
        Uri publicUrl = await publicStorage.SaveAsync(key, stream, cancellationToken);
        stream.Position = 0;
        return UploadedItem.Create(fileSize, fileName, hash, backupUrl, publicUrl);
    }

}
