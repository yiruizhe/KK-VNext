using FileService.Domain;

namespace FileService.Infrasructure.Services;

public class UpCloudStorageClient : IStorageClient
{
    public StorageType StorageType => StorageType.Public;

    public Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
