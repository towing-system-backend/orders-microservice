namespace orders_microservice.Application.Commands.UpdateOrder.types;

public record UpdateOrderCommand(
    string Id,
    string? Status,
    string? TowDriverAssigned,
    string? Destination
);

    
