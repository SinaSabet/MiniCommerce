using Payment.Application.Interfaces;
using Payment.Domain.Common.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace Payment.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly PaymentDbContext _context;
    private readonly IDomainEventDispatcher _dispatcher;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        PaymentDbContext context,
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

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DispatchDomainEventsAsync(
        CancellationToken cancellationToken = default)
    {
        var entries = _context.ChangeTracker
            .Entries()
            .Where(entry => entry.Entity is not null)
            .ToList();

        var events = new List<IDomainEvent>();

        foreach (var entry in entries)
        {
            if (entry.Entity is IHasDomainEvents entity)
            {
                events.AddRange(entity.DomainEvents);
            }
        }

        foreach (var domainEvent in events)
        {
            await _dispatcher.DispatchAsync(domainEvent, cancellationToken);
        }
    }

    public void ClearDomainEvents()
    {
        var entries = _context.ChangeTracker
            .Entries()
            .Where(entry => entry.Entity is not null)
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.Entity is IHasDomainEvents entity)
            {
                entity.ClearDomainEvents();
            }
        }
    }
}
