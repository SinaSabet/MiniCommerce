using BuildingBlocks.Contracts.Events.Inventory;
using BuildingBlocks.Contracts.Events.Ordering;
using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;

namespace Ordering.Infrastructure.Saga;


public sealed class OrderStateMachine
    : MassTransitStateMachine<OrderSagaState>
{

    public State AwaitingInventory { get; private set; } = default!;


    public State AwaitingPayment { get; private set; } = default!;


    public State Completed { get; private set; } = default!;


    public State Failed { get; private set; } = default!;

    public State Compensating { get; private set; } = default!;


    public State Cancelled { get; private set; } = default!;



    public Event<OrderConfirmedIntegrationEvent> OrderConfirmed { get; private set; } = default!;


    public Event<InventoryReservedIntegrationEvent> InventoryReserved { get; private set; } = default!;


    public Event<InventoryReservationFailedIntegrationEvent> InventoryFailed { get; private set; } = default!;


    public Event<PaymentCompletedIntegrationEvent> PaymentCompleted { get; private set; } = default!;


    public Event<PaymentFailedIntegrationEvent> PaymentFailed { get; private set; } = default!;

    public Event<InventoryReleasedIntegrationEvent> InventoryReleased { get; private set; } = default!;


    public OrderStateMachine()
    {

        InstanceState(x => x.CurrentState);


        Event(() => OrderConfirmed,
            x =>
            {
                x.CorrelateById(
                    context => context.Message.OrderId);
            });



        Event(() => InventoryReserved,
            x =>
            {
                x.CorrelateById(
                    context => context.Message.OrderId);
            });



        Event(() => InventoryFailed,
            x =>
            {
                x.CorrelateById(
                    context => context.Message.OrderId);
            });



        Event(() => PaymentCompleted,
            x =>
            {
                x.CorrelateById(
                    context => context.Message.OrderId);
            });



        Event(() => PaymentFailed,
            x =>
            {
                x.CorrelateById(
                    context => context.Message.OrderId);
            });



        Event(() => InventoryReleased,
            x =>
            {
                x.CorrelateById(
                    context => context.Message.OrderId);
            });





        Initially(

            When(OrderConfirmed)


            .Then(context =>
            {

                context.Saga.CorrelationId =
                    context.Message.OrderId;


                context.Saga.OrderId =
                    context.Message.OrderId;


                context.Saga.Amount =
                    context.Message.Amount;


                context.Saga.Currency =
                    context.Message.Currency;


                context.Saga.CreatedAt =
                    DateTime.UtcNow;


                context.Saga.InventoryReserved =
                    false;


                context.Saga.CompensationStarted =
                    false;


                context.Saga.InventoryReleased =
                    false;


                context.Saga.PaymentCompleted =
                    false;

            })



            .Publish(context =>
                new ReserveInventoryRequestedIntegrationEvent
                {
                    OrderId = context.Message.OrderId,
                    Items = context.Message.Items
                        .Select(x =>
                            new ReserveInventoryItem(
                                x.ProductId,
                                x.Quantity))
                        .ToArray()
                })



            .TransitionTo(AwaitingInventory)

        );







        During(

            AwaitingInventory,


            When(InventoryReserved)


            .Then(context =>
            {
                context.Saga.InventoryReserved = true;


                context.Saga.ReservationIds =
                    GetReservationIds(
                        context.Message.ReservationIds,
                        context.Message.ReservationId);
            })



            .Publish(context =>
                new PaymentRequestedIntegrationEvent(
                    context.Saga.OrderId,
                    context.Saga.Amount,
                    context.Saga.Currency))



            .TransitionTo(AwaitingPayment)

        );








        During(

            AwaitingPayment,


            When(PaymentCompleted)


            .Then(context =>
            {

                context.Saga.PaymentCompleted = true;


                context.Saga.CompletedAt =
                    DateTime.UtcNow;

            })



            .TransitionTo(Completed)

        );








        DuringAny(

            When(InventoryFailed)


            .Then(context =>
            {
                context.Saga.CompletedAt =
                    DateTime.UtcNow;
            })


            .TransitionTo(Failed)

        );







        During(

            AwaitingPayment,

            When(PaymentFailed)


            .Then(context =>
            {
                context.Saga.CompensationStarted = true;


                context.Saga.FailedAt =
                    DateTime.UtcNow;
            })


            .Publish(context =>
                new ReleaseInventoryRequestedIntegrationEvent
                {
                    OrderId = context.Saga.OrderId,
                    ReservationIds = context.Saga.ReservationIds.ToArray(),
                    ReservationId = context.Saga.ReservationIds.FirstOrDefault()
                })


            .TransitionTo(Compensating)

        );





        During(

            Compensating,

            When(InventoryReleased)


            .Then(context =>
            {
                var releasedReservationIds =
                    GetReservationIds(
                        context.Message.ReservationIds,
                        context.Message.ReservationId);


                if (context.Saga.ReservationIds.Any(
                    reservationId =>
                        !releasedReservationIds.Contains(reservationId)))
                {
                    throw new InvalidOperationException(
                        "Inventory release confirmation does not contain all reservations for the order.");
                }


                context.Saga.InventoryReleased = true;


                context.Saga.CompletedAt =
                    DateTime.UtcNow;
            })


            .TransitionTo(Cancelled)

        );

    }


    private static List<Guid> GetReservationIds(
        IReadOnlyCollection<Guid> reservationIds,
        Guid reservationId)
    {
        return reservationIds.Count > 0
            ? reservationIds.Distinct().ToList()
            : reservationId == Guid.Empty
                ? new List<Guid>()
                : new List<Guid> { reservationId };
    }

}
