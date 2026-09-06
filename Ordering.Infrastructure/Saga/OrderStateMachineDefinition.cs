using MassTransit;

namespace Ordering.Infrastructure.Saga;

public sealed class OrderStateMachineDefinition
    : SagaDefinition<OrderSagaState>
{
    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<OrderSagaState> sagaConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r =>
        {
            r.Exponential(
                retryLimit: 5,
                minInterval: TimeSpan.FromSeconds(1),
                maxInterval: TimeSpan.FromSeconds(30),
                intervalDelta: TimeSpan.FromSeconds(5));
        });


        endpointConfigurator.UseEntityFrameworkOutbox<OrderSagaDbContext>(
            context);
    }
}