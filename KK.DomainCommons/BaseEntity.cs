using KK.Commons;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace KK.DomainCommons
{
    public class BaseEntity : IEntity, IDomainEvents
    {
        public string Id { get; protected set; } = IdHelper.NextId;

        [NotMapped]
        private List<INotification> domainEvents = new();

        public void AddDomainEvent(INotification eventItem)
        {
            domainEvents.Add(eventItem);
        }

        public void AddDomainEventIfAbsent(INotification eventItem)
        {
            if (!domainEvents.Contains(eventItem))
            {
                domainEvents.Add(eventItem);
            }
        }

        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }

        public IEnumerable<INotification> GetDomainEvents()
        {
            return domainEvents;
        }
    }
}
