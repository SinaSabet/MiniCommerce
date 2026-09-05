using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace Ordering.Infrastructure.Saga;

public class OrderSagaDbContext
    : SagaDbContext
{

    public OrderSagaDbContext(
        DbContextOptions<OrderSagaDbContext> options)
        : base(options)
    {
    }


    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new OrderSagaStateMap();
        }
    }


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddTransactionalOutboxEntities(
            entity =>
                entity.ToTable(
                    $"Saga{entity.Metadata.GetDefaultTableName()}"));
    }
}
