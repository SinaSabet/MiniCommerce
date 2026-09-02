using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Interfaces;
using Payment.Domain.DomainEvents;

namespace Payment.Application.EventHandlers;

public sealed class PaymentCompletedEventHandler
    : IDomainEventHandler<PaymentCompletedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentCompletedEventHandler> _logger;

    public PaymentCompletedEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentCompletedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task HandleAsync(
        PaymentCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new PaymentCompletedIntegrationEvent
            {
                OrderId = domainEvent.OrderId,
                PaymentId = domainEvent.PaymentId,
                Amount = 0 // This would be set from the aggregate, but we need to fetch it
            },
            cancellationToken);

        _logger.LogInformation(
            "PaymentCompleted integration event published to MassTransit Outbox. OrderId: {OrderId}, PaymentId: {PaymentId}",
            domainEvent.OrderId,
            domainEvent.PaymentId);
    }
}
