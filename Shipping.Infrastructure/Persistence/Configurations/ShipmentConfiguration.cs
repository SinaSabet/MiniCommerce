using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipping.Domain.Shipments;

namespace Shipping.Infrastructure.Persistence.Configurations;

public sealed class ShipmentConfiguration
    : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(shipment => shipment.Id);

        builder.Property(shipment => shipment.OrderId)
            .IsRequired();

        builder.Property(shipment => shipment.PaymentId)
            .IsRequired();

        builder.Property(shipment => shipment.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(shipment => shipment.CreatedAt)
            .IsRequired();

        builder.Property(shipment => shipment.ShippedAt)
            .IsRequired(false);

        builder.HasIndex(shipment => shipment.OrderId)
            .IsUnique();
    }
}
