using MediatR;

namespace Payment.Application.Payments.Commands.RefundPayment;

public sealed record RefundPaymentCommand(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount) : IRequest<RefundPaymentCommandResponse>;
