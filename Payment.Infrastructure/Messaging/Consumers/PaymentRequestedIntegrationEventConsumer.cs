using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payment.Application.Payments.Commands.ProcessPayment;

namespace Payment.Infrastructure.Messaging.Consumers;

public sealed class PaymentRequestedIntegrationEventConsumer
    : IConsumer<PaymentRequestedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentRequestedIntegrationEventConsumer> _logger;

    public PaymentRequestedIntegrationEventConsumer(
        IMediator mediator,
        ILogger<PaymentRequestedIntegrationEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentRequestedIntegrationEvent> context)
    {
        _logger.LogInformation(
            "Payment received PaymentRequestedIntegrationEvent. " +
            "OrderId: {OrderId}, Amount: {Amount}, Currency: {Currency}, MessageId: {MessageId}",
            context.Message.OrderId,
            context.Message.Amount,
            context.Message.Currency,
            context.MessageId);

        try
        {
            var command = new ProcessPaymentCommand(
                context.Message.OrderId,
                context.Message.Amount,
                context.Message.Currency);

            var response = await _mediator.Send(command, context.CancellationToken);

            _logger.LogInformation(
                "Payment processed successfully. OrderId: {OrderId}, PaymentId: {PaymentId}",
                context.Message.OrderId,
                response.PaymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing payment for OrderId: {OrderId}",
                context.Message.OrderId);
            throw;
        }
    }
}
