using Microsoft.EntityFrameworkCore;
using Shipping.Domain.Shipments;

namespace Shipping.Infrastructure.Persistence.Repositories;

public sealed class ShipmentRepository : IShipmentRepository
{
    private readonly ShippingDbContext _context;

    public ShipmentRepository(ShippingDbContext context)
    {
        _context = context;
    }

    public Task<Shipment?> GetByIdAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default)
    {
        return _context.Shipments.SingleOrDefaultAsync(
            shipment => shipment.Id == shipmentId,
            cancellationToken);
    }

    public Task<Shipment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return _context.Shipments.SingleOrDefaultAsync(
            shipment => shipment.OrderId == orderId,
            cancellationToken);
    }

    public async Task AddAsync(
        Shipment shipment,
        CancellationToken cancellationToken = default)
    {
        await _context.Shipments.AddAsync(shipment, cancellationToken);
    }
}
