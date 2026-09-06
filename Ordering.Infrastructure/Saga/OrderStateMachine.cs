using BuildingBlocks.Contracts.Events.Inventory;
using BuildingBlocks.Contracts.Events.Ordering;
using BuildingBlocks.Contracts.Events.Payment;
using BuildingBlocks.Contracts.Events.Shipping;
using MassTransit;

namespace Ordering.Infrastructure.Saga;


public sealed class OrderStateMachine
    : MassTransitStateMachine<OrderSagaState>
{

    public State AwaitingInventory { get; private set; } = default!;


    public State AwaitingPayment { get; private set; } = default!;


    public State AwaitingShipping { get; private set; } = default!;


    public State AwaitingRefund { get; private set; } = default!;


    public State Compensating { get; private set; } = default!;


    public State Completed { get; private set; } = default!;


    public State Failed { get; private set; } = default!;


    public State Cancelled { get; private set; } = default!;



    public Event<OrderConfirmedIntegrationEvent> OrderConfirmed { get; private set; } = default!;


    public Event<InventoryReservedIntegrationEvent> InventoryReserved { get; private set; } = default!;


    public Event<InventoryReservationFailedIntegrationEvent> InventoryFailed { get; private set; } = default!;


    public Event<PaymentCompletedIntegrationEvent> PaymentCompleted { get; private set; } = default!;


    public Event<PaymentFailedIntegrationEvent> PaymentFailed { get; private set; } = default!;


    public Event<PaymentRefundedIntegrationEvent> PaymentRefunded { get; private set; } = default!;


    public Event<PaymentRefundFailedIntegrationEvent> PaymentRefundFailed { get; private set; } = default!;


    public Event<InventoryReleasedIntegrationEvent> InventoryReleased { get; private set; } = default!;


    public Event<Fault<PaymentRequestedIntegrationEvent>> PaymentFaulted { get; private set; } = default!;


    public Event<Fault<ReserveInventoryRequestedIntegrationEvent>> InventoryFaulted { get; private set; } = default!;


    public Event<ShipmentCreatedIntegrationEvent> ShipmentCreated { get; private set; } = default!;


    public Event<ShippingFailedIntegrationEvent> ShippingFailed { get; private set; } = default!;


    // Internal Saga timeout event
    public Event<PaymentTimeoutExpired> PaymentTimeoutExpired { get; private set; } = default!;



    public Schedule<OrderSagaState, PaymentTimeoutExpired> PaymentTimeout { get; private set; } = default!;



    public OrderStateMachine()
    {

        InstanceState(
            x => x.CurrentState);



        Schedule(
            () => PaymentTimeout,
            x => x.PaymentTimeoutTokenId,
            x =>
            {
                x.Delay =
                    TimeSpan.FromMinutes(5);
            });



        Event(() => OrderConfirmed,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => InventoryReserved,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => InventoryFailed,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => PaymentCompleted,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => PaymentFailed,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => PaymentRefunded,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => PaymentRefundFailed,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => InventoryReleased,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => ShipmentCreated,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => ShippingFailed,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => PaymentTimeoutExpired,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.OrderId);
            });



        Event(() => PaymentFaulted,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.Message.OrderId);
            });



        Event(() => InventoryFaulted,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.Message.OrderId);
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


                context.Saga.PaymentCompleted =
                    false;


                context.Saga.PaymentRefunded =
                    false;


                context.Saga.CompensationStarted =
                    false;


                context.Saga.InventoryReleased =
                    false;

            })


            .Publish(context =>
                new ReserveInventoryRequestedIntegrationEvent
                {
                    OrderId =
                        context.Message.OrderId,


                    Items =
                        context.Message.Items
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


            When(InventoryFailed)


            .Then(context =>
            {

                context.Saga.FailedAt =
                    DateTime.UtcNow;

            })


            .TransitionTo(Failed),


            When(InventoryFaulted)


            .Then(context =>
            {

                context.Saga.FailedAt =
                    DateTime.UtcNow;

            })


            .TransitionTo(Failed)

        );





        During(

            AwaitingInventory,


            When(InventoryReserved)


            .Then(context =>
            {

                context.Saga.InventoryReserved =
                    true;


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


            .Schedule(
                PaymentTimeout,
                context =>
                    new PaymentTimeoutExpired(
                        context.Saga.OrderId))


            .TransitionTo(AwaitingPayment)

        );
        During(

    AwaitingPayment,


    When(PaymentCompleted)


    .Unschedule(PaymentTimeout)


    .Then(context =>
    {

        context.Saga.PaymentCompleted =
            true;


        context.Saga.PaymentId =
            context.Message.PaymentId;


        context.Saga.CompletedAt =
            DateTime.UtcNow;

    })


    .Publish(context =>
        new ShippingRequestedIntegrationEvent
        {
            OrderId =
                context.Saga.OrderId,


            PaymentId =
                context.Message.PaymentId
        })


    .TransitionTo(AwaitingShipping)

);






        During(

            AwaitingShipping,


            When(ShipmentCreated)


            .Then(context =>
            {

                context.Saga.CompletedAt =
                    DateTime.UtcNow;

            })


            .TransitionTo(Completed)

        );







        During(

            AwaitingShipping,


            When(ShippingFailed)


            .Then(context =>
            {

                context.Saga.CompensationStarted =
                    true;


                context.Saga.FailedAt =
                    DateTime.UtcNow;

            })


            .Publish(context =>
                new RefundPaymentRequestedIntegrationEvent
                {
                    OrderId =
                        context.Saga.OrderId,


                    PaymentId =
                        context.Saga.PaymentId
                        ?? throw new InvalidOperationException(
                            "Cannot compensate shipping without a payment identifier."),


                    Amount =
                        context.Saga.Amount
                })


            .TransitionTo(AwaitingRefund)

        );





        During(

            AwaitingRefund,


            When(PaymentRefunded)


            .Then(context =>
            {

                EnsurePaymentMatches(
                    context.Saga.PaymentId,
                    context.Message.PaymentId);


                context.Saga.PaymentRefunded =
                    true;

            })


            .Publish(context =>
                new ReleaseInventoryRequestedIntegrationEvent
                {
                    OrderId =
                        context.Saga.OrderId,


                    ReservationIds =
                        context.Saga.ReservationIds
                        .ToArray(),


                    ReservationId =
                        context.Saga.ReservationIds
                        .FirstOrDefault()
                })


            .TransitionTo(Compensating),


            When(PaymentRefundFailed)


            .Then(context =>
            {

                EnsurePaymentMatches(
                    context.Saga.PaymentId,
                    context.Message.PaymentId);


                context.Saga.FailedAt =
                    DateTime.UtcNow;

            })


            .TransitionTo(Failed)

        );







        During(

            AwaitingPayment,


            When(PaymentFailed)


            .Then(context =>
            {

                context.Saga.CompensationStarted =
                    true;


                context.Saga.FailedAt =
                    DateTime.UtcNow;

            })


            .Publish(context =>
                new ReleaseInventoryRequestedIntegrationEvent
                {
                    OrderId =
                        context.Saga.OrderId,


                    ReservationIds =
                        context.Saga.ReservationIds
                        .ToArray(),


                    ReservationId =
                        context.Saga.ReservationIds
                        .FirstOrDefault()

                })


            .TransitionTo(Compensating)

        );





        During(

            AwaitingPayment,


            When(PaymentFaulted)


            .Then(context =>
            {

                context.Saga.CompensationStarted =
                    true;


                context.Saga.FailedAt =
                    DateTime.UtcNow;

            })


            .Publish(context =>
                new ReleaseInventoryRequestedIntegrationEvent
                {
                    OrderId =
                        context.Saga.OrderId,


                    ReservationIds =
                        context.Saga.ReservationIds
                        .ToArray(),


                    ReservationId =
                        context.Saga.ReservationIds
                        .FirstOrDefault()
                })


            .TransitionTo(Compensating)

        );







        During(

            AwaitingPayment,

            When(PaymentTimeoutExpired)


            .Then(context =>
            {

                context.Saga.CompensationStarted =
                    true;


                context.Saga.FailedAt =
                    DateTime.UtcNow;

            })


            .Publish(context =>
                new ReleaseInventoryRequestedIntegrationEvent
                {
                    OrderId =
                        context.Saga.OrderId,


                    ReservationIds =
                        context.Saga.ReservationIds
                        .ToArray(),


                    ReservationId =
                        context.Saga.ReservationIds
                        .FirstOrDefault()

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
                        "Inventory release confirmation does not contain all reservations.");

                }



                context.Saga.InventoryReleased =
                    true;


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

            ? reservationIds
                .Distinct()
                .ToList()


            : reservationId == Guid.Empty

                ? new List<Guid>()


                : new List<Guid>
                {
                    reservationId
                };

    }



    private static void EnsurePaymentMatches(
        Guid? expectedPaymentId,
        Guid actualPaymentId)
    {
        if (expectedPaymentId is null || expectedPaymentId != actualPaymentId)
            throw new InvalidOperationException(
                "Payment compensation confirmation does not match the saga payment.");
    }

}
