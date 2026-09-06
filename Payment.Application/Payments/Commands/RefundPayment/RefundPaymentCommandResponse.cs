namespace Payment.Application.Payments.Commands.RefundPayment;

public sealed record RefundPaymentCommandResponse(
    Guid PaymentId,
    bool Refunded,
    bool AlreadyRefunded,
    string? FailureReason);
