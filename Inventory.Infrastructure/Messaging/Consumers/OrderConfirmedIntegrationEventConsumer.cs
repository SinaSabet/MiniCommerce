using Inventory.Application.Inventory.Commands.ReserveOrderInventory;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Contracts.IntegrationEvents;

namespace Inventory.Infrastructure.Messaging.Consumers;

public sealed class OrderConfirmedIntegrationEventConsumer
    : IConsumer<OrderConfirmedIntegrationEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<OrderConfirmedIntegrationEventConsumer> _logger;

    public OrderConfirmedIntegrationEventConsumer(
        ISender sender,
        ILogger<OrderConfirmedIntegrationEventConsumer> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(
        ConsumeContext<OrderConfirmedIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "OrderConfirmedIntegrationEvent received. " +
            "OrderId: {OrderId}, MessageId: {MessageId}, ItemsCount: {ItemsCount}",
            message.OrderId,
            context.MessageId,
            message.Items.Count);

        var items = message.Items
            .Select(x => new ReserveOrderInventoryItem(
                x.ProductId,
                x.Quantity))
            .ToList();

        var command = new ReserveOrderInventoryCommand(
            message.OrderId,
            items);

        var result = await _sender.Send(
            command,
            context.CancellationToken);

        _logger.LogInformation(
            "Order inventory reservation completed. " +
            "OrderId: {OrderId}, ReservedItemsCount: {ReservedItemsCount}, AlreadyReserved: {AlreadyReserved}",
            result.OrderId,
            result.ReservedItemsCount,
            result.AlreadyReserved);
    }
}