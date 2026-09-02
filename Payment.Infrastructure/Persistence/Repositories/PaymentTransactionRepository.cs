using Payment.Domain.PaymentTransactions;
using Microsoft.EntityFrameworkCore;

namespace Payment.Infrastructure.Persistence.Repositories;

public sealed class PaymentTransactionRepository : IPaymentTransactionRepository
{
    private readonly PaymentDbContext _context;

    public PaymentTransactionRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentTransaction?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentTransactions
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<PaymentTransaction?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentTransactions
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId,
                cancellationToken);
    }

    public async Task AddAsync(
        PaymentTransaction paymentTransaction,
        CancellationToken cancellationToken = default)
    {
        await _context.PaymentTransactions.AddAsync(
            paymentTransaction,
            cancellationToken);
    }

    public async Task UpdateAsync(
        PaymentTransaction paymentTransaction,
        CancellationToken cancellationToken = default)
    {
        _context.PaymentTransactions.Update(paymentTransaction);
        await Task.CompletedTask;
    }
}
