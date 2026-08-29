using BuildingBlocks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Contracts.IntegrationEvents
{
    public sealed record OrderCreatedIntegrationEvent: IntegrationEvent
    {
        public Guid OrderId { get; init; }

        public Guid ProductId { get; init; }

        public int Quantity { get; init; }

    }
}
