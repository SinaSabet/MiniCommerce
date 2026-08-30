using MassTransit;
using Ordering.Application.Interfaces;
using Ordering.Contracts.IntegrationEvents;
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
            new OrderConfirmedIntegrationEvent
            {
                OrderId = domainEvent.OrderId,

                Items = domainEvent.Items
                    .Select(x => new OrderItemMessage
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity
                    })
                    .ToList()
            };


        await _publishEndpoint.Publish(
            integrationEvent,
            cancellationToken);
    }
}