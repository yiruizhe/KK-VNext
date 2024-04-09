using FileService.Domain.Entities;

namespace FileService.Domain;
public interface IFSRepository
{
    /// <summary>
    /// 查找已经上传的相同大小以及散列值的文件
    /// </summary>
    /// <param name="fileSize"></param>
    /// <param name="fileSha256Hash"></param>
    /// <returns></returns>
    Task<UploadedItem?> FindAsync(long fileSize, string fileSha256Hash);
}
