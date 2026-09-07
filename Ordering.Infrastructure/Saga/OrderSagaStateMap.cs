using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Infrastructure.Saga;


public class OrderSagaStateMap
    : SagaClassMap<OrderSagaState>
{

    protected override void Configure(
        EntityTypeBuilder<OrderSagaState> entity,
        ModelBuilder model)
    {

        entity.ToTable("OrderSagas");


        entity.HasKey(
            x => x.CorrelationId);



        // Optimistic Concurrency Token
        entity.Property(
            x => x.RowVersion)
            .IsRowVersion();



        entity.Property(
            x => x.CurrentState)
            .HasMaxLength(64)
            .IsRequired();



        entity.Property(
            x => x.Currency)
            .HasMaxLength(10);



        // Faster business lookup/debugging
        entity.HasIndex(
            x => x.OrderId);



        // Helps queries on active sagas
        entity.HasIndex(
            x => x.CurrentState);



        entity.Property(
            x => x.Amount)
            .HasPrecision(
                18,
                2);

    }
}