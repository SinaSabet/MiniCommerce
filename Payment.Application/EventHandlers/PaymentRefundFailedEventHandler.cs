using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Interfaces;
using Payment.Domain.DomainEvents;

namespace Payment.Application.EventHandlers;

public sealed class PaymentRefundFailedEventHandler
    : IDomainEventHandler<PaymentRefundFailedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentRefundFailedEventHandler> _logger;

    public PaymentRefundFailedEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentRefundFailedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task HandleAsync(
        PaymentRefundFailedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new PaymentRefundFailedIntegrationEvent
            {
                OrderId = domainEvent.OrderId,
                PaymentId = domainEvent.PaymentId,
                Reason = domainEvent.Reason
            },
            cancellationToken);

        _logger.LogWarning(
            "PaymentRefundFailed integration event added to the outbox. OrderId: {OrderId}, PaymentId: {PaymentId}, Reason: {Reason}",
            domainEvent.OrderId,
            domainEvent.PaymentId,
            domainEvent.Reason);
    }
}
