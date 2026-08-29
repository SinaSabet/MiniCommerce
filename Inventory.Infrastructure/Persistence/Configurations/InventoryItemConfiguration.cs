using Inventory.Domain.InventoryItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public sealed class InventoryItemConfiguration
    : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(
        EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.HasIndex(x => x.ProductId)
            .IsUnique();

        builder.Property(x => x.OnHandQuantity)
            .IsRequired();

        builder.Property(x => x.ReservedQuantity)
            .IsRequired();


        builder.Ignore(x => x.AvailableQuantity);


        // برای جلوگیری از Lost Update
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
    }
}