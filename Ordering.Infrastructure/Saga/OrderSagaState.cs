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



    public List<Guid> ReservationIds { get; set; } = new();



    public bool PaymentCompleted { get; set; }


    public Guid? PaymentTimeoutTokenId { get; set; }



    public DateTime CreatedAt { get; set; }



    public DateTime? CompletedAt { get; set; }


    public bool CompensationStarted { get; set; }

    public bool InventoryReleased { get; set; }


    public DateTime? FailedAt { get; set; }

}
