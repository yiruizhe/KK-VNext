namespace MediaEncoder.Domain.Entities
{
    public enum ItemStatus
    {
        Ready,//任务开始创建
        Started,//开始处理
        Completed,//处理成功
        Failed// 失败
    }
}
