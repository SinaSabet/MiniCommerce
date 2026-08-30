using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Contracts.IntegrationEvents
{
    public sealed class InventoryReservationFailedIntegrationEvent
    {
        public Guid OrderId { get; init; }

        public DateTime ReservedAtUtc { get; init; }

        public string Reason { get; set; }
    }
}
