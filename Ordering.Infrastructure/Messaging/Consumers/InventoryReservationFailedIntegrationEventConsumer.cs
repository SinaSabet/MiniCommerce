using BuildingBlocks.Contracts.Events.Inventory;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Orders.Commands.MarkInventoryReservationFailed;

namespace Ordering.Infrastructure.Messaging.Consumers;

public sealed class InventoryReservationFailedIntegrationEventConsumer
    : IConsumer<InventoryReservationFailedIntegrationEvent>
{
    private readonly ISender _sender;

    private readonly ILogger<
        InventoryReservationFailedIntegrationEventConsumer> _logger;


    public InventoryReservationFailedIntegrationEventConsumer(
        ISender sender,
        ILogger<InventoryReservationFailedIntegrationEventConsumer> logger)
    {
        _sender = sender;
        _logger = logger;
    }


    public async Task Consume(
        ConsumeContext<InventoryReservationFailedIntegrationEvent> context)
    {
        var message = context.Message;


        _logger.LogWarning(
            "InventoryReservationFailedIntegrationEvent received. " +
            "OrderId: {OrderId}, Reason: {Reason}, MessageId: {MessageId}",
            message.OrderId,
            message.Reason,
            context.MessageId);


        await _sender.Send(
            new MarkInventoryReservationFailedCommand(
                message.OrderId,
                message.Reason),
            context.CancellationToken);
    }
}