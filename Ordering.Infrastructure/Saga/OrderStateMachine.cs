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

    public Event<ShipmentCreatedIntegrationEvent> ShipmentCreated { get; private set; } = default!;


    public Event<ShippingFailedIntegrationEvent> ShippingFailed { get; private set; } = default!;


    public Schedule<OrderSagaState, PaymentTimeoutExpired> PaymentTimeout { get; private set; } = default!;


    public Schedule<OrderSagaState, ShippingTimeoutExpired> ShippingTimeout { get; private set; } = default!;


    public Event<Fault<ReserveInventoryRequestedIntegrationEvent>> InventoryFaulted { get; private set; } = default!;


    public Event<Fault<PaymentRequestedIntegrationEvent>> PaymentFaulted { get; private set; } = default!;


    public Event<Fault<ShippingRequestedIntegrationEvent>> ShippingRequestedFault { get; private set; } = default!;


    public Event<Fault<RefundPaymentRequestedIntegrationEvent>> RefundPaymentRequestedFault { get; private set; } = default!;

    public Event<Fault<ReleaseInventoryRequestedIntegrationEvent>> ReleaseInventoryFaulted {get;private set;} = default!;

    public OrderStateMachine()
    {

        InstanceState(
            x => x.CurrentState);



        Schedule(
     () => PaymentTimeout,
     x => x.PaymentTimeoutTokenId,
     x =>
     {
         x.Delay = TimeSpan.FromMinutes(5);

         x.Received = e =>
             e.CorrelateById(context => context.Message.OrderId);
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



        Schedule(
      () => ShippingTimeout,
      x => x.ShippingTimeoutTokenId,
      x =>
      {
          x.Delay = TimeSpan.FromHours(24);

          x.Received = e =>
              e.CorrelateById(context => context.Message.OrderId);
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

        Event(() => ReleaseInventoryFaulted,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.Message.OrderId);
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



        Event(() => ShippingRequestedFault,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.Message.OrderId);
            });


        Event(() => RefundPaymentRequestedFault,
            x =>
            {
                x.CorrelateById(
                    context =>
                        context.Message.Message.OrderId);
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

             .IfElse(
                 context => !context.Saga.PaymentCompleted,


                 then => then

                     .Unschedule(PaymentTimeout)

                     .Then(context =>
                     {
                         context.Saga.PaymentCompleted = true;

                         context.Saga.PaymentId =
                             context.Message.PaymentId;

                     })

                     .Publish(context =>
                         new ShippingRequestedIntegrationEvent
                         {
                             OrderId =
                                 context.Saga.OrderId,

                             PaymentId =
                                 context.Message.PaymentId
                         })


                     .Schedule(
                         ShippingTimeout,
                         context =>
                             new ShippingTimeoutExpired(
                                 context.Saga.OrderId))


                     .TransitionTo(AwaitingShipping),


                 otherwise => otherwise
             )

         );






        During(

            AwaitingShipping,


            When(ShipmentCreated)


            .IfElse(
                context => context.Saga.CompletedAt is null,


                then => then

                    .Unschedule(ShippingTimeout)

                    .Then(context =>
                    {
                        context.Saga.CompletedAt =
                            DateTime.UtcNow;
                    })


                    .TransitionTo(Completed),


                otherwise => otherwise
            )

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

         .IfElse(
             context => !context.Saga.PaymentRefunded,


             then => then

                 .Then(context =>
                 {
                     context.Saga.PaymentRefunded = true;
                 })


                 .Publish(context =>
                     new ReleaseInventoryRequestedIntegrationEvent
                     {
                         OrderId =
                             context.Saga.OrderId,

                         ReservationIds =
                             context.Saga.ReservationIds.ToArray(),

                         ReservationId =
                             context.Saga.ReservationIds.FirstOrDefault()
                     })


                 .TransitionTo(Compensating),


             otherwise => otherwise
         )

     );


            During(

                Compensating,

                When(ReleaseInventoryFaulted)

                .Then(context =>
                {
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

            AwaitingShipping,


            When(ShippingTimeout.Received)


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
                            "Cannot compensate shipping timeout without a payment identifier."),

                    Amount =
                        context.Saga.Amount
                })


            .TransitionTo(AwaitingRefund)

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

            When(PaymentTimeout.Received)


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

             .IfElse(
                 context => !context.Saga.InventoryReleased,


                 then => then

                     .Then(context =>
                     {
                         context.Saga.InventoryReleased = true;

                         context.Saga.CompletedAt =
                             DateTime.UtcNow;
                     })


                     .TransitionTo(Cancelled),


                 otherwise => otherwise
             )

         );

        During(

        AwaitingShipping,


        When(ShippingRequestedFault)


        .Then(context =>
        {
            context.Saga.CompensationStarted = true;

            context.Saga.FailedAt =
                DateTime.UtcNow;
        })


        .Publish(context =>
            new RefundPaymentRequestedIntegrationEvent
            {
                OrderId =
                    context.Saga.OrderId,

                PaymentId =
                    context.Saga.PaymentId!.Value,

                Amount =
                    context.Saga.Amount
            })


        .TransitionTo(AwaitingRefund)

    );

                During(

            AwaitingRefund,


            When(RefundPaymentRequestedFault)


            .Then(context =>
            {
                context.Saga.FailedAt =
                    DateTime.UtcNow;
            })


            .TransitionTo(Failed)

        );



        During(
            AwaitingShipping,
            Ignore(PaymentCompleted));


        During(
            AwaitingRefund,
            Ignore(PaymentCompleted));


        During(
            Compensating,
            Ignore(PaymentCompleted),
            Ignore(PaymentRefunded));


        During(
            Completed,
            Ignore(PaymentCompleted),
            Ignore(ShipmentCreated));


        During(
            Failed,
            Ignore(PaymentCompleted),
            Ignore(PaymentRefunded));


        During(
            Cancelled,
            Ignore(PaymentCompleted),
            Ignore(PaymentRefunded),
            Ignore(InventoryReleased));

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
}
