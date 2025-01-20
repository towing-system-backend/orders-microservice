namespace Order.Infrastructure
{
    public record FindOrderByIdResponse
    (
        string Id,
        string Status,
        string IssueLocation,
        string Destination,
        string Issuer,
        string? TowDriverAssigned,
        string Details,
        string Name,
        string? Image,
        string? Policy,
        string PhoneNumber,
        decimal TotalCost,
        double TotalDistance,
        int IdentificationNumber,
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