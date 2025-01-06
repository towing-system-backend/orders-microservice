namespace Order.Infrastructure
{
    public record FindOrderByStatusResponse
    (
        string Id,
        string Status,
        string IssueLocation,
        string Destination,
        string? TowDriverAssigned,
        string Details,
        string Name,
        string? Image,
        string? Policy,
        string PhoneNumber,
        decimal TotalCost,
        List<AdditonalCostResponse> AdditionalCosts
    );

    public record AdditonalCostResponse
    (
        string Id,
        string Name,
        string Category,
        decimal? Amount
    );
}