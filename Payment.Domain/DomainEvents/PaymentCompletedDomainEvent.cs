using Payment.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DomainEvents
{
    public record PaymentCompletedDomainEvent(Guid PaymentId, Guid OrderId) : DomainEvent
    {
    }
}
