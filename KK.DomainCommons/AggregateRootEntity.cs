namespace KK.DomainCommons
{
    public class AggregateRootEntity : BaseEntity, IAggregateRoot, IHasCreateTime, IHasDeleteTime, ISoftDelete, IHasModifyTime
    {
        public DateTime? ModifyTime { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime? DeleteTime { get; private set; }

        public DateTime CreateTime { get; init; } = DateTime.Now;

        public virtual void SoftDelete()
        {
            IsDeleted = true;
            DeleteTime = DateTime.Now;
        }

        public virtual void NotifyModify()
        {
            ModifyTime = DateTime.Now;
        }
    }
}
