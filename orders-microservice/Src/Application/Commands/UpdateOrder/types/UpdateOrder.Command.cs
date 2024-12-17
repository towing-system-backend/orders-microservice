namespace orders_microservice.Application.Commands.UpdateOrder.types;

public record UpdateOrderCommand(
    string Id,
    string? status,
    string? towDriverAssigned,
    string? destination
);

    
