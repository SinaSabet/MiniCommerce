using MediatR;
using Payment.Domain.PaymentTransactions;

namespace Payment.Application.Payments.Commands.CompletePayment;

public sealed class CompletePaymentCommandHandler
    : IRequestHandler<CompletePaymentCommand, CompletePaymentCommandResponse>
{
    private readonly IPaymentTransactionRepository _paymentRepository;

    public CompletePaymentCommandHandler(
        IPaymentTransactionRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<CompletePaymentCommandResponse> Handle(
        CompletePaymentCommand command,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository
            .GetByIdAsync(command.PaymentId, cancellationToken);

        if (payment == null)
            throw new InvalidOperationException($"Payment with id {command.PaymentId} not found");

        payment.Complete();

        await _paymentRepository
            .UpdateAsync(payment, cancellationToken);

        return new CompletePaymentCommandResponse(
            payment.Id,
            "Payment completed successfully");
    }
}
