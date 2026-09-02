using BuildingBlocks.Contracts.Events.Ordering;
using MassTransit;
using Ordering.Application.Interfaces;
using Ordering.Domain.Events;


namespace Ordering.Application.EventHandlers;


public sealed class OrderConfirmedEventHandler
    : IDomainEventHandler<OrderConfirmedEvent>
{

    private readonly IPublishEndpoint _publishEndpoint;



    public OrderConfirmedEventHandler(
        IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }




    public async Task HandleAsync(
        OrderConfirmedEvent domainEvent,
        CancellationToken cancellationToken)
    {


        var integrationEvent =
            new OrderConfirmedIntegrationEvent(

                domainEvent.OrderId,


                domainEvent.Amount,


                domainEvent.Currency,


                domainEvent.Items
                    .Select(x =>
                        new BuildingBlocks.Contracts.Events.Ordering.OrderConfirmedItem(
                            x.ProductId,
                            x.Quantity))
                    .ToList()

            );



        await _publishEndpoint.Publish(
            integrationEvent,
            cancellationToken);

    }

}