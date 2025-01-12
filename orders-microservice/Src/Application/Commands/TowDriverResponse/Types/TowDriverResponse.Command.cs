namespace Order.Application
{
    public record TowDriverResponseCommand
    (
        string OrderId,
        string? Status,
        string? Response
    );
}
