using Inventory.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class InventoryReservationRepository
    : IInventoryReservationRepository
{
    private readonly InventoryDbContext _context;

    public InventoryReservationRepository(
        InventoryDbContext context)
    {
        _context = context;
    }


    public Task<InventoryReservation?>
        GetByOrderAndProductAsync(
            Guid orderId,
            Guid productId,
            CancellationToken cancellationToken = default)
    {
        return _context.InventoryReservations
            .SingleOrDefaultAsync(
                x =>
                    x.OrderId == orderId &&
                    x.ProductId == productId,
                cancellationToken);
    }


    public async Task AddAsync(
        InventoryReservation reservation,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryReservations.AddAsync(
            reservation,
            cancellationToken);
    }

    public Task<InventoryReservation?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}