using BuildingBlocks.Contracts.Events.Ordering;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Messaging.Consumers;

public sealed class OrderCreatedIntegrationEventConsumer
    : IConsumer<OrderCreatedIntegrationEvent>
{
    private readonly ILogger<OrderCreatedIntegrationEventConsumer> _logger;

    public OrderCreatedIntegrationEventConsumer(
        ILogger<OrderCreatedIntegrationEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(
        ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        _logger.LogInformation(
            "Inventory received OrderCreatedIntegrationEvent. " +
            "OrderId: {OrderId}, MessageId: {MessageId}",
            context.Message.OrderId,
            context.MessageId);

        return Task.CompletedTask;
    }
}