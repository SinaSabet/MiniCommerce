using MediatR;

namespace Shipping.Application.Shipments.Commands.CreateShipment;

public sealed record CreateShipmentCommand(Guid OrderId,Guid PaymentId)
    : IRequest<ShipmentDto>;
