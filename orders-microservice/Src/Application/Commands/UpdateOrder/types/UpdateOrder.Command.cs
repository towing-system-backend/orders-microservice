namespace Order.Application
{
    public record AdditionalCostCommand(
        string Name,
        string Category,
        decimal Amount
    );

    public record UpdateOrderCommand(
        string Id,
        string? Status,
        string? TowDriverAssigned,
        string? Destination,
        List<AdditionalCostCommand> AdditionalCosts
    );
}