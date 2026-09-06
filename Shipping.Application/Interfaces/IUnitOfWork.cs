namespace Shipping.Application.Interfaces;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task DispatchDomainEventsAsync(
        CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(
        CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default);

    void ClearDomainEvents();
}
