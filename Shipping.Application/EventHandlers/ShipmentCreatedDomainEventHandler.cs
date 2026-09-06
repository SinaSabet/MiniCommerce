using BuildingBlocks.Contracts.Events.Shipping;
using MassTransit;
using Shipping.Application.Interfaces;
using Shipping.Domain.DomainEvents;

namespace Shipping.Application.EventHandlers;

public sealed class ShipmentCreatedDomainEventHandler
    : IDomainEventHandler<ShipmentCreatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public ShipmentCreatedDomainEventHandler(
        IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task HandleAsync(
        ShipmentCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        return _publishEndpoint.Publish(
            new ShipmentCreatedIntegrationEvent
            {
                OrderId = domainEvent.OrderId,
                ShipmentId = domainEvent.ShipmentId
            },
            cancellationToken);
    }
}
