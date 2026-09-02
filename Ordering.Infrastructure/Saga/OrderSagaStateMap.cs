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


        entity.Property(
            x => x.CurrentState)
            .HasMaxLength(64);


        entity.Property(
            x => x.Currency)
            .HasMaxLength(10);

    }
}