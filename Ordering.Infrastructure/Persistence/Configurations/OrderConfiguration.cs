using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(
        EntityTypeBuilder<Order> builder)
        {

            builder.ToTable("Orders");


            builder.HasKey(x => x.Id);



            builder.Property(x => x.Status)
                .HasConversion<int>();



            builder.OwnsOne(
                x => x.Number,
                number =>
                {
                    number.Property(x => x.Value)
                        .HasColumnName("OrderNumber")
                        .HasMaxLength(50);
                });



            builder.OwnsOne(
                x => x.ShippingAddress,
                address =>
                {
                    address.Property(x => x.City)
                        .HasMaxLength(100);


                    address.Property(x => x.Street)
                        .HasMaxLength(200);


                    address.Property(x => x.PostalCode)
                        .HasMaxLength(20);
                });



            builder.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
