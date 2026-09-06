using MediatR;

namespace Shipping.Application.Shipments.Queries.GetShipment;

public sealed record GetShipmentQuery(Guid ShipmentId)
    : IRequest<ShipmentDto?>;
