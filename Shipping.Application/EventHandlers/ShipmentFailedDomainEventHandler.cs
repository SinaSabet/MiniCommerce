using BuildingBlocks.Contracts.Events.Shipping;
using MassTransit;
using Shipping.Application.Interfaces;
using Shipping.Domain.DomainEvents;

namespace Shipping.Application.EventHandlers;

public sealed class ShipmentFailedDomainEventHandler
    : IDomainEventHandler<ShipmentFailedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public ShipmentFailedDomainEventHandler(
        IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task HandleAsync(
        ShipmentFailedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        return _publishEndpoint.Publish(
            new ShippingFailedIntegrationEvent
            {
                OrderId = domainEvent.OrderId,
                Reason = domainEvent.Reason
            },
            cancellationToken);
    }
}
