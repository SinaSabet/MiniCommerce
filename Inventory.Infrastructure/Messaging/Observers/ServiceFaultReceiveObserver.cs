using BuildingBlocks.Contracts.Events.Monitoring;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Messaging.Observers;

public sealed class ServiceFaultReceiveObserver : IReceiveObserver
{
    private const string ServiceName = "Inventory";

    private readonly ILogger<ServiceFaultReceiveObserver> _logger;

    public ServiceFaultReceiveObserver(
        ILogger<ServiceFaultReceiveObserver> logger)
    {
        _logger = logger;
    }

    public Task PreReceive(ReceiveContext context) => Task.CompletedTask;

    public Task PostReceive(ReceiveContext context) => Task.CompletedTask;

    public Task PostConsume<T>(
        ConsumeContext<T> context,
        TimeSpan duration,
        string consumerType)
        where T : class => Task.CompletedTask;

    public async Task ConsumeFault<T>(
        ConsumeContext<T> context,
        TimeSpan duration,
        string consumerType,
        Exception exception)
        where T : class
    {
        try
        {
            await context.Publish(
                new ServiceFaultDetectedIntegrationEvent
                {
                    ServiceName = ServiceName,
                    MessageType = typeof(T).FullName ?? typeof(T).Name,
                    ExceptionType = exception.GetType().FullName
                        ?? exception.GetType().Name,
                    ExceptionMessage = exception.Message,
                    CorrelationId = context.CorrelationId,
                    MessageId = context.MessageId,
                    Timestamp = DateTime.UtcNow
                },
                context.CancellationToken);
        }
        catch (Exception reportingException)
        {
            _logger.LogError(
                reportingException,
                "Unable to publish fault monitoring event. ServiceName: {ServiceName}, " +
                "MessageType: {MessageType}, MessageId: {MessageId}",
                ServiceName,
                typeof(T).FullName ?? typeof(T).Name,
                context.MessageId);
        }
    }

    public Task ReceiveFault(
        ReceiveContext context,
        Exception exception) => Task.CompletedTask;
}
