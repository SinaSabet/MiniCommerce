using Shipping.Application.Interfaces;
using Shipping.Domain.Common.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shipping.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ShippingDbContext _context;
    private readonly IDomainEventDispatcher _dispatcher;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        ShippingDbContext context,
        IDomainEventDispatcher dispatcher)
    {
        _context = context;
        _dispatcher = dispatcher;
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DispatchDomainEventsAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = _context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToArray();

        if (domainEvents.Length == 0)
            return;

        await _dispatcher.DispatchAsync(
            domainEvents,
            cancellationToken);
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void ClearDomainEvents()
    {
        var aggregateRoots = _context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity);

        foreach (var aggregateRoot in aggregateRoots)
            aggregateRoot.ClearDomainEvents();
    }
}
