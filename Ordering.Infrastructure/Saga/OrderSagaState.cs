using MassTransit;

namespace Ordering.Infrastructure.Saga;


public sealed class OrderSagaState
    : SagaStateMachineInstance
{

    public Guid CorrelationId { get; set; }


    public string CurrentState { get; set; } = default!;



    public Guid OrderId { get; set; }



    public decimal Amount { get; set; }



    public string Currency { get; set; } = default!;



    public bool InventoryReserved { get; set; }



    public bool PaymentCompleted { get; set; }



    public DateTime CreatedAt { get; set; }



    public DateTime? CompletedAt { get; set; }

}