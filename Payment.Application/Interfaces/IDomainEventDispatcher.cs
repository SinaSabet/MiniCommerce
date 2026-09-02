using Payment.Domain.Common.Events;

namespace Payment.Application.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(
            IDomainEvent domainEvent,
            CancellationToken cancellationToken = default);
    }
}
