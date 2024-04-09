
using KK.DomainCommons;
using MediaEncoder.Domain.Events;

namespace MediaEncoder.Domain.Entities
{
    public class EncodingItem : BaseEntity, IAggregateRoot, IHasCreateTime
    {
        public DateTime CreateTime { get; private set; }
        public string SourceSystem { get; private set; }
        /// <summary>
        /// 文件大小，字节
        /// </summary>
        public long? FileSizeInBytes { get; private set; }
        /// <summary>
        /// 文件名，不包含路径
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// 文件的散列值（SAH256）
        /// </summary>
        public string? FileSHA256Hash { get; private set; }

        /// <summary>
        /// 待转码文件
        /// </summary>
        public Uri SourceUrl { get; private set; }

        /// <summary>
        /// 转码完成的路径
        /// </summary>
        public Uri? OutputUrl { get; private set; }

        /// <summary>
        /// 转码目标类型
        /// </summary>
        public string OutputFormat { get; private set; }
        /// <summary>
        /// 转码工具的输出日志
        /// </summary>
        public string? LogText { get; private set; }
        /// <summary>
        /// 当前转码任务状态
        /// </summary>
        public ItemStatus Status;
        /// <summary>
        /// 创建该转码任务的用户
        /// </summary>
        public string CreateUser { get; private set; }

        private EncodingItem() { }

        /// <summary>
        /// 开始转码任务
        /// </summary>
        public void Start()
        {
            this.Status = ItemStatus.Started;
            //发布任务开始处理的领域事件
            AddDomainEvent(new EncodingItemStartedEvent(this.Id, SourceSystem, CreateUser));
        }
        /// <summary>
        /// 任务处理成功
        /// </summary>
        public void Complete(Uri outputUrl)
        {
            this.Status = ItemStatus.Completed;
            this.OutputUrl = outputUrl;
            this.LogText = "转码成功";
            AddDomainEvent(new EncodingItemCompletdEvent(Id, SourceSystem, outputUrl, CreateUser));
        }

        public void Fail(string logText)
        {
            this.Status = ItemStatus.Failed;
            this.LogText = logText;
            AddDomainEventIfAbsent(new EncodingItemFailedEvent(Id, SourceSystem, logText, CreateUser));
        }

        public void Fail(Exception ex)
        {
            Fail($"任务处理失败:{ex}");
        }

        public void ChangeFileMetaData(long fileSize, string fileHash)
        {
            this.FileSizeInBytes = fileSize;
            this.FileSHA256Hash = fileHash;
        }

        public static EncodingItem Create(string id, string fileName, Uri sourceUrl, string outputFormat,
            string sourceSystem, string userId)
        {
            return new EncodingItem()
            {
                Id = id,
                FileName = fileName,
                SourceSystem = sourceSystem,
                CreateTime = DateTime.Now,
                SourceUrl = sourceUrl,
                OutputFormat = outputFormat,
                CreateUser = userId,
                Status = ItemStatus.Ready
            };
        }
    }
}
