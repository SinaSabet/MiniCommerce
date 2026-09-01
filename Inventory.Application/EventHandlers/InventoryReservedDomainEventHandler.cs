using BuildingBlocks.Contracts.Events.Inventory;
using Inventory.Domain.DomainEvents;
using MassTransit;
using MediatR;

public class InventoryReservedDomainEventHandler
    : INotificationHandler<InventoryReservedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;


    public InventoryReservedDomainEventHandler(
        IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }


    public async Task Handle(
        InventoryReservedDomainEvent notification,
        CancellationToken cancellationToken)
    {

        await _publishEndpoint.Publish(
            new InventoryReservedIntegrationEvent(
                notification.OrderId,
                notification.ReservationId,
                []
            ),
            cancellationToken);
    }
}