using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shipping.Application.Interfaces;
using Shipping.Domain.Common.Events;

namespace Shipping.Application.Services;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            _logger.LogInformation(
                "Dispatching domain event {EventType} with EventId {EventId}",
                domainEvent.GetType().Name,
                domainEvent.EventId);

            var handlerType = typeof(IDomainEventHandler<>)
                .MakeGenericType(domainEvent.GetType());

            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                await ((dynamic)handler!).HandleAsync(
                    (dynamic)domainEvent,
                    cancellationToken);
            }
        }
    }
}
