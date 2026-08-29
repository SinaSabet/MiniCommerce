using Inventory.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.DomainEvents
{
    public sealed record InventoryReservationReleasedEvent(
     Guid ReservationId,
     Guid OrderId,
     Guid ProductId,
     int Quantity)
     : DomainEvent;
}
