using Inventory.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(
      IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken);
    }
}
