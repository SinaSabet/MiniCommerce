using Ordering.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Events
{
    public record OrderCreatedEvent(Guid OrderId) : DomainEvent
    {
    }
}
