using Ordering.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(
      IEnumerable<IDomainEvent> domainEvents,CancellationToken cancellationToken);
    }
}
