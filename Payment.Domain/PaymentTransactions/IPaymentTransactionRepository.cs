namespace Payment.Domain.PaymentTransactions;

public interface IPaymentTransactionRepository
{
    Task<PaymentTransaction?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PaymentTransaction?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PaymentTransaction paymentTransaction,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        PaymentTransaction paymentTransaction,
        CancellationToken cancellationToken = default);
}
