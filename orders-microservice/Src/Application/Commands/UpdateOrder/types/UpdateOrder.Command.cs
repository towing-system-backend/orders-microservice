namespace orders_microservice.Application.Commands.UpdateOrder.types;

public record UpdateOrderCommand(
    string Id,
    string? Status,
    string? TowDriverAssigned,
    string? Destination,
    List<AdditionalCostCommand> AdditionalCosts
);

public record AdditionalCostCommand
(
    string Name,
    string Category,
    decimal Amount
);

    
