using MediatR;

namespace Payment.Application.Payments.Commands.CompletePayment;

public sealed record CompletePaymentCommand(
    Guid PaymentId) : IRequest<CompletePaymentCommandResponse>
{
}
