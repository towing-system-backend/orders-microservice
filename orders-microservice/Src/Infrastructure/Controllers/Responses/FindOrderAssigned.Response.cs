namespace Order.Infrastructure
{
    public record FindOrderAssignedResponse
    (
        string Status,
        string IssueLocation,
        string Destination,
        string Details,
        string Name,
        string? Image,
        string PhoneNumber,
        string? TowDriverAssigned
    );
}