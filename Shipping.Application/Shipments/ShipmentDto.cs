using Shipping.Domain.Shipments;

namespace Shipping.Application.Shipments;

public sealed record ShipmentDto(
    Guid Id,
    Guid OrderId,
    Guid PaymentId,
    ShipmentStatus Status,
    DateTime CreatedAt,
    DateTime? ShippedAt)
{
    public static ShipmentDto FromShipment(Shipment shipment)
    {
        return new ShipmentDto(
            shipment.Id,
            shipment.OrderId,
            shipment.PaymentId,
            shipment.Status,
            shipment.CreatedAt,
            shipment.ShippedAt);
    }
}
