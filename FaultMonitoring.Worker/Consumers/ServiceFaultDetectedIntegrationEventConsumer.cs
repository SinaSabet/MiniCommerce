using BuildingBlocks.Contracts.Events.Monitoring;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FaultMonitoring.Worker.Consumers;

public sealed class ServiceFaultDetectedIntegrationEventConsumer
    : IConsumer<ServiceFaultDetectedIntegrationEvent>
{
    private readonly ILogger<ServiceFaultDetectedIntegrationEventConsumer> _logger;

    public ServiceFaultDetectedIntegrationEventConsumer(
        ILogger<ServiceFaultDetectedIntegrationEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(
        ConsumeContext<ServiceFaultDetectedIntegrationEvent> context)
    {
        var fault = context.Message;

        _logger.LogError(
            "Service fault detected. ServiceName: {ServiceName}, MessageType: {MessageType}, " +
            "ExceptionType: {ExceptionType}, ExceptionMessage: {ExceptionMessage}, " +
            "CorrelationId: {CorrelationId}, MessageId: {MessageId}, Timestamp: {Timestamp}",
            fault.ServiceName,
            fault.MessageType,
            fault.ExceptionType,
            fault.ExceptionMessage,
            fault.CorrelationId,
            fault.MessageId,
            fault.Timestamp);

        return Task.CompletedTask;
    }
}
