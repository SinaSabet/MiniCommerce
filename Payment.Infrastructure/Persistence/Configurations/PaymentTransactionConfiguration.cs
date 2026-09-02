using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.PaymentTransactions;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Persistence.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CompletedAt)
            .IsRequired(false);

        // Configure Money value object
        builder.OwnsOne(
            x => x.Amount,
            money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(19, 4)
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        // Index for OrderId for quick lookups
        builder.HasIndex(x => x.OrderId)
            .IsUnique();
    }
}
