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
        endpointConfigurator.UseEntityFrameworkOutbox<OrderSagaDbContext>(
            context);
    }
}
