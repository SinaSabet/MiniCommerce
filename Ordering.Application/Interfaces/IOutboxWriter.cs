using BuildingBlocks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Interfaces
{
    public interface IOutboxWriter
    {
        Task AddAsync( IntegrationEvent integrationEvent,CancellationToken cancellationToken = default);
    }
}
