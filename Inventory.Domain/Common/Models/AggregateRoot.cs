using Inventory.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Common.Models
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents where TId : notnull
    {
        private readonly List<IDomainEvent> _events = new();
        protected AggregateRoot(TId id) : base(id)
        {
        }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _events.Add(domainEvent);
        }



        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _events.Remove(domainEvent);
        }



        public void ClearDomainEvents()
        {
            _events.Clear();
        }
    }
}
