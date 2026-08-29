namespace Ordering.Application.Interfaces
{
    public interface IUnitOfWork
    {

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);



        Task BeginTransactionAsync(
            CancellationToken cancellationToken = default);



        Task CommitTransactionAsync(
            CancellationToken cancellationToken = default);



        Task RollbackTransactionAsync(
            CancellationToken cancellationToken = default);



        Task DispatchDomainEventsAsync(
            CancellationToken cancellationToken = default);


        void ClearDomainEvents();


    }
}