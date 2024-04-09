using MediatR;

namespace KK.DomainCommons
{
    public interface IDomainEvents
    {
        IEnumerable<INotification> GetDomainEvents();
        void AddDomainEvent(INotification eventItem);
        void AddDomainEventIfAbsent(INotification eventItem);
        public void ClearDomainEvents();
    }
}
