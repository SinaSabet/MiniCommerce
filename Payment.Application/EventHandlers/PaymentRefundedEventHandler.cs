using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Interfaces;
using Payment.Domain.DomainEvents;

namespace Payment.Application.EventHandlers;

public sealed class PaymentRefundedEventHandler
    : IDomainEventHandler<PaymentRefundedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentRefundedEventHandler> _logger;

    public PaymentRefundedEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentRefundedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task HandleAsync(
        PaymentRefundedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new PaymentRefundedIntegrationEvent
            {
                OrderId = domainEvent.OrderId,
                PaymentId = domainEvent.PaymentId
            },
            cancellationToken);

        _logger.LogInformation(
            "PaymentRefunded integration event added to the outbox. OrderId: {OrderId}, PaymentId: {PaymentId}",
            domainEvent.OrderId,
            domainEvent.PaymentId);
    }
}
