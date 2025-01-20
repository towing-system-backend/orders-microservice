namespace Order.Infrastructure
{
    public record FindAllOrdersResponse
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
}