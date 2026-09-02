using MediatR;
using Payment.Domain.PaymentTransactions;

namespace Payment.Application.Payments.Commands.ProcessPayment;

public sealed class ProcessPaymentCommandHandler
    : IRequestHandler<ProcessPaymentCommand, ProcessPaymentCommandResponse>
{
    private readonly IPaymentTransactionRepository _paymentRepository;

    public ProcessPaymentCommandHandler(
        IPaymentTransactionRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<ProcessPaymentCommandResponse> Handle(
        ProcessPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var payment = Domain.PaymentTransactions.PaymentTransaction.Create(
            command.OrderId,
            command.Amount,
            command.Currency);

        await _paymentRepository
            .AddAsync(payment, cancellationToken);

        return new ProcessPaymentCommandResponse(
            payment.Id,
            "Payment processed successfully");
    }
}
