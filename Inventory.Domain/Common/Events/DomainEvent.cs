using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Common.Events
{
    public abstract record DomainEvent:IDomainEvent
    {
        public Guid EventId { get; }= Guid.NewGuid();



        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
