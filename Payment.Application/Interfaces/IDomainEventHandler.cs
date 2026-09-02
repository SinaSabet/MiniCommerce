using Payment.Domain.Common.Events;

namespace Payment.Application.Interfaces
{
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
    }
}
