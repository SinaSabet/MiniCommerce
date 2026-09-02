using Payment.Domain.PaymentTransactions;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace Payment.Infrastructure.Persistence;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(
        DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    public DbSet<PaymentTransaction> PaymentTransactions =>
        Set<PaymentTransaction>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PaymentDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
