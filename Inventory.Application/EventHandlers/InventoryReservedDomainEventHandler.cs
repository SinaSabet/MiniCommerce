using BuildingBlocks.Contracts.Events.Inventory;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainEvents;
using MassTransit;
using MediatR;
namespace Inventory.Application.EventHandlers
{

    public class InventoryReservedDomainEventHandler
        : IDomainEventHandler<InventoryReservedDomainEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;


        public InventoryReservedDomainEventHandler(
            IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }


        public async Task HandleAsync(
       InventoryReservedDomainEvent domainEvent,
       CancellationToken cancellationToken)
        {
            var items = domainEvent.Items
                .Select(x =>
                    new BuildingBlocks.Contracts.Events.Inventory.InventoryReservedItem(
                        x.ProductId,
                        x.Quantity))
                .ToList();


            var integrationEvent =
                new InventoryReservedIntegrationEvent(
                    domainEvent.OrderId,
                    domainEvent.ReservationId,
                    items);


            await _publishEndpoint.Publish(
                integrationEvent,
                cancellationToken);
        }
    }
}