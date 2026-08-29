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
    public class OrderItemConfiguration: IEntityTypeConfiguration<OrderItem>
    {

        public void Configure(
            EntityTypeBuilder<OrderItem> builder)
        {

            builder.ToTable("OrderItems");


            builder.HasKey(x => x.Id);



            builder.Property(x => x.ProductName)
                .HasMaxLength(200);



            builder.OwnsOne(
                x => x.UnitPrice,
                money =>
                {
                    money.Property(x => x.Amount)
                        .HasColumnName("UnitPrice");


                    money.Property(x => x.Currency)
                        .HasColumnName("Currency")
                        .HasMaxLength(3);
                });

        }

    }
}
