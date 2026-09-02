using MediatR;

namespace Payment.Application.Payments.Commands.FailPayment;

public sealed record FailPaymentCommand(
    Guid PaymentId,
    string Reason) : IRequest<FailPaymentCommandResponse>
{
}
