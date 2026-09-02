using MediatR;
using Payment.Domain.PaymentTransactions;

namespace Payment.Application.Payments.Commands.FailPayment;

public sealed class FailPaymentCommandHandler
    : IRequestHandler<FailPaymentCommand, FailPaymentCommandResponse>
{
    private readonly IPaymentTransactionRepository _paymentRepository;

    public FailPaymentCommandHandler(
        IPaymentTransactionRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<FailPaymentCommandResponse> Handle(
        FailPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository
            .GetByIdAsync(command.PaymentId, cancellationToken);

        if (payment == null)
            throw new InvalidOperationException($"Payment with id {command.PaymentId} not found");

        payment.Fail(command.Reason);

        await _paymentRepository
            .UpdateAsync(payment, cancellationToken);

        return new FailPaymentCommandResponse(
            payment.Id,
            "Payment failed successfully");
    }
}
