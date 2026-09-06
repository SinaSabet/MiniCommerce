using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using MediatR;
using Payment.Domain.Common.Exceptions;
using Payment.Domain.PaymentTransactions;

namespace Payment.Application.Payments.Commands.RefundPayment;

public sealed class RefundPaymentCommandHandler
    : IRequestHandler<RefundPaymentCommand, RefundPaymentCommandResponse>
{
    private readonly IPaymentTransactionRepository _paymentRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public RefundPaymentCommandHandler(
        IPaymentTransactionRepository paymentRepository,
        IPublishEndpoint publishEndpoint)
    {
        _paymentRepository = paymentRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<RefundPaymentCommandResponse> Handle(
        RefundPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository
            .GetByIdAsync(command.PaymentId, cancellationToken);

        if (payment is null)
        {
            var reason = $"Payment with id {command.PaymentId} not found";
            await PublishFailureAsync(command, reason, cancellationToken);

            return new RefundPaymentCommandResponse(
                command.PaymentId,
                false,
                false,
                reason);
        }

        if (payment.OrderId != command.OrderId)
        {
            const string reason = "Payment does not belong to the requested order";
            await PublishFailureAsync(command, reason, cancellationToken);

            return new RefundPaymentCommandResponse(
                payment.Id,
                false,
                false,
                reason);
        }

        try
        {
            var refunded = payment.Refund(command.Amount);

            if (refunded)
                await _paymentRepository.UpdateAsync(payment, cancellationToken);

            return new RefundPaymentCommandResponse(
                payment.Id,
                refunded,
                !refunded,
                null);
        }
        catch (DomainException exception)
        {
            payment.RecordRefundFailure(exception.Message);
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            return new RefundPaymentCommandResponse(
                payment.Id,
                false,
                false,
                exception.Message);
        }
    }

    private Task PublishFailureAsync(
        RefundPaymentCommand command,
        string reason,
        CancellationToken cancellationToken)
    {
        return _publishEndpoint.Publish(
            new PaymentRefundFailedIntegrationEvent
            {
                OrderId = command.OrderId,
                PaymentId = command.PaymentId,
                Reason = reason
            },
            cancellationToken);
    }
}
