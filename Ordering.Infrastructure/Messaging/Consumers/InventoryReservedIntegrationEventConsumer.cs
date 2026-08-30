using Inventory.Contracts.IntegrationEvents;
using MassTransit;
using MediatR;
using Ordering.Application.Orders.Commands.MarkInventoryReserved;

namespace Ordering.Infrastructure.Messaging.Consumers;

public sealed class InventoryReservedIntegrationEventConsumer
    : IConsumer<InventoryReservedIntegrationEvent>
{
    private readonly ISender _sender;


    public InventoryReservedIntegrationEventConsumer(
        ISender sender)
    {
        _sender = sender;
    }


    public async Task Consume(
        ConsumeContext<InventoryReservedIntegrationEvent> context)
    {
        await _sender.Send(
            new MarkInventoryReservedCommand(
                context.Message.OrderId),
            context.CancellationToken);
    }
}