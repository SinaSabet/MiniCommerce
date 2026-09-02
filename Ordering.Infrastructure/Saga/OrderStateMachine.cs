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



    public Event<OrderConfirmedIntegrationEvent> OrderConfirmed { get; private set; } = default!;


    public Event<InventoryReservedIntegrationEvent> InventoryReserved { get; private set; } = default!;


    public Event<InventoryReservationFailedIntegrationEvent> InventoryFailed { get; private set; } = default!;


    public Event<PaymentCompletedIntegrationEvent> PaymentCompleted { get; private set; } = default!;


    public Event<PaymentFailedIntegrationEvent> PaymentFailed { get; private set; } = default!;




    public OrderStateMachine()
    {


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

            })



            .PublishAsync(context =>
                context.Init<ReserveInventoryRequestedIntegrationEvent>(
                    new
                    {

                        OrderId =
                            context.Message.OrderId,


                        Items =
                            context.Message.Items
                            .Select(x =>
                                new ReserveInventoryItem(
                                    x.ProductId,
                                    x.Quantity))
                            .ToList()

                    }))



            .TransitionTo(AwaitingInventory)

        );







        During(

            AwaitingInventory,


            When(InventoryReserved)


            .Then(context =>
            {
                context.Saga.InventoryReserved = true;
            })



            .PublishAsync(context =>
                context.Init<PaymentRequestedIntegrationEvent>(
                    new
                    {

                        OrderId =
                            context.Saga.OrderId,


                        Amount =
                            context.Saga.Amount,


                        Currency =
                            context.Saga.Currency

                    }))



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







        DuringAny(

            When(PaymentFailed)


            .Then(context =>
            {
                context.Saga.CompletedAt =
                    DateTime.UtcNow;
            })


            .TransitionTo(Failed)

        );

    }

}