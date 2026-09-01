using Inventory.Application.Interfaces;
using Inventory.Domain.Common.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventory.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;
    private readonly IDomainEventDispatcher _dispatcher;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        InventoryDbContext context, IDomainEventDispatcher dispatcher)
    {
        _context = context;
        _dispatcher = dispatcher;
    }


    public async Task BeginTransactionAsync(
         CancellationToken cancellationToken = default)
    {

        _transaction =
            await _context.Database
            .BeginTransactionAsync(
                cancellationToken);

    }



    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {

        if (_transaction is not null)
        {
            await _transaction.CommitAsync(
                cancellationToken);


            await _transaction.DisposeAsync();

            _transaction = null;
        }

    }



    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {

        if (_transaction is not null)
        {

            await _transaction.RollbackAsync(
                cancellationToken);


            await _transaction.DisposeAsync();

            _transaction = null;
        }

    }



    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {

        return await _context.SaveChangesAsync(
            cancellationToken);

    }



    public async Task DispatchDomainEventsAsync(
        CancellationToken cancellationToken = default)
    {

        var domainEvents =
            GetDomainEvents();


        if (!domainEvents.Any())
            return;



        await _dispatcher.DispatchAsync(
            domainEvents,
            cancellationToken);


    }



    private List<IDomainEvent> GetDomainEvents()
    {

        return _context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

    }



    public void ClearDomainEvents()
    {

        var entities =
        _context.ChangeTracker
        .Entries<IHasDomainEvents>()
        .Select(x => x.Entity);


        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }

    }

}