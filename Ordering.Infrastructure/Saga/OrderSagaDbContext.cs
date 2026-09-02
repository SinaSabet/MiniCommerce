using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

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
}