using BuildingBlocks.Contracts.Events.Shipping;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipping.Application.Shipments.Commands.CreateShipment;

namespace Shipping.Infrastructure.Messaging.Consumers;

public sealed class ShippingRequestedIntegrationEventConsumer
    : IConsumer<ShippingRequestedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShippingRequestedIntegrationEventConsumer> _logger;

    public ShippingRequestedIntegrationEventConsumer(
        IMediator mediator,
        ILogger<ShippingRequestedIntegrationEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    public async Task Consume(
        ConsumeContext<ShippingRequestedIntegrationEvent> context)
    {
        _logger.LogInformation(
            "Shipping request received. OrderId: {OrderId}, PaymentId: {PaymentId}, MessageId: {MessageId}",
            context.Message.OrderId,
            context.Message.PaymentId,
            context.MessageId);

        try
        {
            var command = new CreateShipmentCommand(
                context.Message.OrderId,
                context.Message.PaymentId);

            var shipment = await _mediator.Send(
                command,
                context.CancellationToken);

            _logger.LogInformation(
                "Shipping request processed. OrderId: {OrderId}, ShipmentId: {ShipmentId}",
                shipment.OrderId,
                shipment.Id);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error processing shipping request. OrderId: {OrderId}, PaymentId: {PaymentId}",
                context.Message.OrderId,
                context.Message.PaymentId);

            throw;
        }
    }
}
