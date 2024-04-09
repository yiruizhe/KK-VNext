using KK.DomainCommons;

namespace FileService.Domain.Entities
{
    public class UploadedItem : BaseEntity, IHasCreateTime
    {
        public DateTime CreateTime { get; private set; }

        public long FileSizeInBytes { get; private set; }

        public string FileName { get; private set; }

        public string FileSHA256Hash { get; private set; }

        public Uri BackupUrl { get; private set; }

        public Uri RemoteUrl { get; private set; }

        public static UploadedItem Create(long fileSizeInBytes, string fileName, string fileSHA256Hash, Uri backUrl, Uri remoteUrl)
        {
            return new UploadedItem
            {
                CreateTime = DateTime.Now,
                FileSizeInBytes = fileSizeInBytes,
                FileName = fileName,
                FileSHA256Hash = fileSHA256Hash,
                BackupUrl = backUrl,
                RemoteUrl = remoteUrl
            };
        }
    }
}
