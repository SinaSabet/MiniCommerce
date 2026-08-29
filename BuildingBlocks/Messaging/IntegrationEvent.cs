using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging
{
    public abstract record IntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();

        public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;

        public string CorrelationId { get; init; } = string.Empty;
    }
}
