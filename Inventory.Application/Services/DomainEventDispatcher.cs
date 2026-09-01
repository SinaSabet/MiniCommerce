using Inventory.Application.Interfaces;
using Inventory.Domain.Common.Events;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(
      IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(
         IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            foreach (var domainEvent in domainEvents)
            {

                var handlerType =
                    typeof(IDomainEventHandler<>)
                    .MakeGenericType(
                        domainEvent.GetType());


                var handlers =
                    _serviceProvider
                    .GetServices(handlerType);



                foreach (var handler in handlers)
                {

                    await ((dynamic)handler)
                        .HandleAsync(
                        (dynamic)domainEvent, cancellationToken);

                }

            }

        }
    }
}
