using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Monitoring;

public sealed record ServiceFaultDetectedIntegrationEvent : IntegrationEvent
{
    public string ServiceName { get; init; } = string.Empty;

    public string MessageType { get; init; } = string.Empty;

    public string ExceptionType { get; init; } = string.Empty;

    public string ExceptionMessage { get; init; } = string.Empty;

    public new Guid? CorrelationId { get; init; }

    public Guid? MessageId { get; init; }

    public DateTime Timestamp { get; init; }
}
