using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Contracts.IntegrationEvents
{
    public sealed record OrderConfirmedIntegrationEvent
    {
        public Guid OrderId { get; init; }

        public List<OrderItemMessage> Items { get; init; } = new();
    }
    public sealed record OrderItemMessage
    {
        public Guid ProductId { get; init; }

        public int Quantity { get; init; }
    }
}
