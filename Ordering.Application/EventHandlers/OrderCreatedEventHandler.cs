using BuildingBlocks.Contracts.Events.Ordering;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Application.Interfaces;
using Ordering.Domain.Events;

namespace Ordering.Application.EventHandlers;

public sealed class OrderCreatedEventHandler
    : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<OrderCreatedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task HandleAsync(
        OrderCreatedEvent domainEvent,
        CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new OrderCreatedIntegrationEvent
            {
                OrderId = domainEvent.OrderId
            },
            cancellationToken);

        _logger.LogInformation(
            "OrderCreated integration event published to MassTransit Outbox. OrderId: {OrderId}",
            domainEvent.OrderId);
    }
}