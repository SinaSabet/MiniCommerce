using Inventory.Domain.InventoryItems;
using Inventory.Domain.Reservations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistence
{
    public class InventoryDbContext: DbContext
    {
        public InventoryDbContext(
       DbContextOptions<InventoryDbContext> options)
       : base(options)
        {
        }


        public DbSet<InventoryItem> InventoryItems =>
            Set<InventoryItem>();


        public DbSet<InventoryReservation> InventoryReservations =>
            Set<InventoryReservation>();


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(InventoryDbContext).Assembly);
        }
    }
}
