namespace orders_microservice.Application.Commands.RegisterOrder.types;

public record RegisterOrderCommand(
    string status,
    string issueLocation,
    string destination,
    string details,
    string name,
    string image,
    string policy,
    string phoneNumber
);