namespace orders_microservice.Application.Commands.RegisterOrder.types;

public record RegisterOrderCommand(
    string Status,
    string IssueLocation,
    string Destination,
    string Details,
    string Name,
    string Image,
    string Policy,
    string PhoneNumber
);