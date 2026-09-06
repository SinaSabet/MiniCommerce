namespace Shipping.Domain.Shipments;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<Shipment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Shipment shipment,
        CancellationToken cancellationToken = default);
}
