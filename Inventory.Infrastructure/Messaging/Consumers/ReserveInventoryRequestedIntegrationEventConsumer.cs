using BuildingBlocks.Contracts.Events.Inventory;
using Inventory.Application.Inventory.Commands.ReserveInventory;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Inventory.Infrastructure.Messaging.Consumers;


public sealed class ReserveInventoryRequestedIntegrationEventConsumer
    : IConsumer<ReserveInventoryRequestedIntegrationEvent>
{


    private readonly ISender _sender;

    private readonly ILogger<ReserveInventoryRequestedIntegrationEventConsumer> _logger;



    public ReserveInventoryRequestedIntegrationEventConsumer(
        ISender sender,
        ILogger<ReserveInventoryRequestedIntegrationEventConsumer> logger)
    {
        _sender = sender;
        _logger = logger;
    }





    public async Task Consume(
        ConsumeContext<ReserveInventoryRequestedIntegrationEvent> context)
    {


        var message = context.Message;



        _logger.LogInformation(
            "ReserveInventoryRequested received. OrderId: {OrderId}",
            message.OrderId);



        var items =
            message.Items
                .Select(x =>
                    new ReserveInventoryCommandItem(
                        x.ProductId,
                        x.Quantity))
                .ToList();



        var command =
            new ReserveInventoryCommand(
                message.OrderId,
                items);



        var result =
            await _sender.Send(
                command,
                context.CancellationToken);



        _logger.LogInformation(
            "Inventory reservation completed. OrderId: {OrderId}, AlreadyReserved: {AlreadyReserved}",
            result.OrderId,
            result.AlreadyReserved);

    }
}