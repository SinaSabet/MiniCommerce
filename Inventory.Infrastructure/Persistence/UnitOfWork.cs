using Inventory.Application.Interfaces;

namespace Inventory.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;

    public UnitOfWork(
        InventoryDbContext context)
    {
        _context = context;
    }


    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }
}