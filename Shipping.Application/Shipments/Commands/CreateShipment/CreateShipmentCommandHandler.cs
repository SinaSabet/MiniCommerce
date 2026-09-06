using MediatR;
using Microsoft.Extensions.Logging;
using Shipping.Domain.Shipments;

namespace Shipping.Application.Shipments.Commands.CreateShipment;

public sealed class CreateShipmentCommandHandler
    : IRequestHandler<CreateShipmentCommand, ShipmentDto>
{
    private readonly IShipmentRepository _repository;
    private readonly ILogger<CreateShipmentCommandHandler> _logger;

    public CreateShipmentCommandHandler(
        IShipmentRepository repository,
        ILogger<CreateShipmentCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ShipmentDto> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var existingShipment = await _repository.GetByOrderIdAsync(
            request.OrderId,
            cancellationToken);

        if (existingShipment is not null)
        {
            _logger.LogInformation(
                "Existing shipment returned. ShipmentId: {ShipmentId}, OrderId: {OrderId}",
                existingShipment.Id,
                existingShipment.OrderId);

            return ShipmentDto.FromShipment(existingShipment);
        }

        var shipment = Shipment.Create(
            request.OrderId,
            request.PaymentId);

        shipment.MarkCreated();

        await _repository.AddAsync(shipment, cancellationToken);

        _logger.LogInformation(
            "Shipment created. ShipmentId: {ShipmentId}, OrderId: {OrderId}, PaymentId: {PaymentId}",
            shipment.Id,
            shipment.OrderId,
            shipment.PaymentId);

        return ShipmentDto.FromShipment(shipment);
    }
}
