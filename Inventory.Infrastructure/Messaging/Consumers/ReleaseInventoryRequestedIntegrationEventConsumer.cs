using BuildingBlocks.Contracts.Events.Inventory;
using Inventory.Application.Inventory.Commands.ReleaseInventory;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Messaging.Consumers;

public sealed class ReleaseInventoryRequestedIntegrationEventConsumer
    : IConsumer<ReleaseInventoryRequestedIntegrationEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<ReleaseInventoryRequestedIntegrationEventConsumer> _logger;

    public ReleaseInventoryRequestedIntegrationEventConsumer(
        ISender sender,
        ILogger<ReleaseInventoryRequestedIntegrationEventConsumer> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(
        ConsumeContext<ReleaseInventoryRequestedIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "ReleaseInventoryRequested received. OrderId: {OrderId}, ReservationCount: {ReservationCount}, MessageId: {MessageId}",
            message.OrderId,
            message.ReservationIds.Count > 0 ? message.ReservationIds.Count : 1,
            context.MessageId);

        var reservationIds =
            message.ReservationIds.Count > 0
                ? message.ReservationIds
                : new[] { message.ReservationId };

        var command = new ReleaseInventoryCommand(
            message.OrderId,
            reservationIds);

        var result = await _sender.Send(
            command,
            context.CancellationToken);

        _logger.LogInformation(
            "Inventory release completed. OrderId: {OrderId}, ReservationCount: {ReservationCount}, AlreadyReleased: {AlreadyReleased}",
            result.OrderId,
            result.ReservationIds.Count,
            result.AlreadyReleased);
    }
}
