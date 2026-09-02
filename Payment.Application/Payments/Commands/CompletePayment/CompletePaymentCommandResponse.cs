namespace Payment.Application.Payments.Commands.CompletePayment;

public sealed record CompletePaymentCommandResponse(
    Guid PaymentId,
    string Message)
{
}
