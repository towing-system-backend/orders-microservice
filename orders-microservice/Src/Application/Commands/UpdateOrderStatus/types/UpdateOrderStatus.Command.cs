namespace orders_microservice.Application.Commands.UpdateOrder.types;

public record UpdateOrderStatusCommand(
    string Id,
    string Status
);

    
