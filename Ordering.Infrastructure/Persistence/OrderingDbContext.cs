using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Ordering.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderingDbContext : DbContext, IOrderingDbContext
    {
        public OrderingDbContext(
    DbContextOptions<OrderingDbContext> options)
    : base(options)
        {
        }
        public DbSet<Order> Orders
    => Set<Order>();


        public DbSet<OrderItem> OrderItems
            => Set<OrderItem>();

        public DatabaseFacade Database
    => base.Database;
        protected override void OnModelCreating(
       ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(OrderingDbContext).Assembly);


            modelBuilder.AddInboxStateEntity();

            modelBuilder.AddOutboxMessageEntity();

            modelBuilder.AddOutboxStateEntity();
        }
    }
}
