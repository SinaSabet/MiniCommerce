using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Interfaces;
using Payment.Domain.DomainEvents;

namespace Payment.Application.EventHandlers;

public sealed class PaymentFailedEventHandler
    : IDomainEventHandler<PaymentFailedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentFailedEventHandler> _logger;

    public PaymentFailedEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentFailedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task HandleAsync(
        PaymentFailedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new PaymentFailedIntegrationEvent
            {
                OrderId = domainEvent.OrderId,
                PaymentId = domainEvent.PaymentId,
                Reason = domainEvent.Reason
            },
            cancellationToken);

        _logger.LogInformation(
            "PaymentFailed integration event published to MassTransit Outbox. OrderId: {OrderId}, PaymentId: {PaymentId}, Reason: {Reason}",
            domainEvent.OrderId,
            domainEvent.PaymentId,
            domainEvent.Reason);
    }
}
