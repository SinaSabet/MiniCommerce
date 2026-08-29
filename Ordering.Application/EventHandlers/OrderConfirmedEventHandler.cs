using MassTransit;
using Ordering.Application.Interfaces;
using Ordering.Contracts.IntegrationEvents;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.EventHandlers
{
    public sealed class OrderConfirmedEventHandler
        : IDomainEventHandler<OrderConfirmedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderConfirmedEventHandler(
            IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task HandleAsync(
            OrderConfirmedEvent domainEvent,
            CancellationToken cancellationToken)
        {
            await _publishEndpoint.Publish(
                new OrderConfirmedIntegrationEvent
                {
                    OrderId = domainEvent.OrderId,
                    Items = domainEvent.Items
                        .Select(x => new OrderItemMessage
                        {
                            ProductId = x.ProductId,
                            Quantity = x.Quantity
                        })
                        .ToList()
                },
                cancellationToken);
        }
    }
}
