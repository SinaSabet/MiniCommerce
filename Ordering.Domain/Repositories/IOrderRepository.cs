using Ordering.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id,CancellationToken cancellationToken);

        Task AddAsync(Order order);

        Task UpdateAsync(Order order);

        Task<bool> ExistsAsync(Guid id);
    }
}
