using MediatR;

namespace Payment.Application.Payments.Commands.ProcessPayment;

public sealed record ProcessPaymentCommand(
    Guid OrderId,
    decimal Amount,
    string Currency) : IRequest<ProcessPaymentCommandResponse>
{
}
