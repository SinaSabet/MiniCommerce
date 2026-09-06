using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payment.Application.Payments.Commands.RefundPayment;

namespace Payment.Infrastructure.Messaging.Consumers;

public sealed class RefundPaymentRequestedIntegrationEventConsumer
    : IConsumer<RefundPaymentRequestedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RefundPaymentRequestedIntegrationEventConsumer> _logger;

    public RefundPaymentRequestedIntegrationEventConsumer(
        IMediator mediator,
        ILogger<RefundPaymentRequestedIntegrationEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(
        ConsumeContext<RefundPaymentRequestedIntegrationEvent> context)
    {
        _logger.LogInformation(
            "Refund requested. OrderId: {OrderId}, PaymentId: {PaymentId}, Amount: {Amount}, MessageId: {MessageId}",
            context.Message.OrderId,
            context.Message.PaymentId,
            context.Message.Amount,
            context.MessageId);

        try
        {
            var response = await _mediator.Send(
                new RefundPaymentCommand(
                    context.Message.OrderId,
                    context.Message.PaymentId,
                    context.Message.Amount),
                context.CancellationToken);

            if (response.FailureReason is not null)
            {
                _logger.LogWarning(
                    "Refund rejected by business rules. OrderId: {OrderId}, PaymentId: {PaymentId}, Reason: {Reason}",
                    context.Message.OrderId,
                    response.PaymentId,
                    response.FailureReason);
                return;
            }

            _logger.LogInformation(
                response.AlreadyRefunded
                    ? "Duplicate refund request ignored. OrderId: {OrderId}, PaymentId: {PaymentId}"
                    : "Payment refunded. OrderId: {OrderId}, PaymentId: {PaymentId}",
                context.Message.OrderId,
                response.PaymentId);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Technical error refunding payment. OrderId: {OrderId}, PaymentId: {PaymentId}",
                context.Message.OrderId,
                context.Message.PaymentId);
            throw;
        }
    }
}
