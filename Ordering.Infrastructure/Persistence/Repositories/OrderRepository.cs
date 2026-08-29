using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Orders;
using Ordering.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly OrderingDbContext _context;



        public OrderRepository(
            OrderingDbContext context)
        {
            _context = context;
        }



        public async Task AddAsync(
            Order order)
        {
            await _context.Orders
                .AddAsync(order);
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Order?> GetByIdAsync(
            Guid orderId)
        {

            return await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(
                    x => x.Id == orderId);

        }



        public Task UpdateAsync(
            Order order)
        {

            _context.Orders
                .Update(order);


            return Task.CompletedTask;

        }

    }
}
