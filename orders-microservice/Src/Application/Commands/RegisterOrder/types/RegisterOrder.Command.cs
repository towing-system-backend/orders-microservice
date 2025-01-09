namespace Order.Application
{
    public record RegisterOrderCommand(
        string Status,
        string IssueLocation,
        string Destination,
        string Issuer,
        string Details,
        string Name,
        string Image,
        string Policy,
        string PhoneNumber,
        int IdentificationNumber
    );
}