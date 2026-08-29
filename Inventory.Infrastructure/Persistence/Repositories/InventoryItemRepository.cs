using Inventory.Domain.InventoryItems;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class InventoryItemRepository
    : IInventoryItemRepository
{
    private readonly InventoryDbContext _context;

    public InventoryItemRepository(
        InventoryDbContext context)
    {
        _context = context;
    }


    public Task<InventoryItem?> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return _context.InventoryItems
            .SingleOrDefaultAsync(
                x => x.ProductId == productId,
                cancellationToken);
    }


    public async Task AddAsync(
        InventoryItem inventoryItem,
        CancellationToken cancellationToken = default)
    {
        await _context.InventoryItems.AddAsync(
            inventoryItem,
            cancellationToken);
    }
}