using Inventory.Application.Inventory.Commands.ReserveInventory;
using MassTransit;
using MediatR;
using Ordering.Contracts.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Messaging.Consumers
{
    public sealed class OrderConfirmedIntegrationEventConsumer
     : IConsumer<OrderConfirmedIntegrationEvent>
    {
        private readonly ISender _sender;

        public OrderConfirmedIntegrationEventConsumer(
            ISender sender)
        {
            _sender = sender;
        }

        public async Task Consume(
            ConsumeContext<OrderConfirmedIntegrationEvent> context)
        {
            foreach (var item in context.Message.Items)
            {
                await _sender.Send(
                    new ReserveInventoryCommand(
                        context.Message.OrderId,
                        item.ProductId,
                        item.Quantity),
                    context.CancellationToken);
            }
        }
    }
}
