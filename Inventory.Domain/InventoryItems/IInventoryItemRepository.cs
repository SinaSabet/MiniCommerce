using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.InventoryItems
{
    public interface IInventoryItemRepository
    {
        Task<InventoryItem?> GetByProductIdAsync(
       Guid productId,
       CancellationToken cancellationToken = default);


        Task AddAsync(
            InventoryItem inventoryItem,
            CancellationToken cancellationToken = default);
    }
}
