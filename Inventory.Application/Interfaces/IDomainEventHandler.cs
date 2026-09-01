using Inventory.Domain.Common.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Interfaces
{
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);

    }
}
