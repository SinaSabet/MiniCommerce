using Payment.Application.Interfaces;
using Payment.Domain.Common.Events;
using Microsoft.Extensions.Logging;

namespace Payment.Infrastructure.Services;

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
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Dispatching domain event {EventType} with EventId {EventId}",
            domainEvent.GetType().Name,
            domainEvent.EventId);

        var handlerType = typeof(IDomainEventHandler<>)
            .MakeGenericType(domainEvent.GetType());

        var handlers = _serviceProvider
            .GetService(typeof(IEnumerable<>).MakeGenericType(handlerType))
            as System.Collections.IEnumerable;

        if (handlers == null)
        {
            _logger.LogWarning(
                "No handlers found for domain event {EventType}",
                domainEvent.GetType().Name);
            return;
        }

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod("HandleAsync");
            if (handleMethod != null)
            {
                await (Task)handleMethod.Invoke(
                    handler,
                    new object[] { domainEvent, cancellationToken })!;
            }
        }
    }
}
