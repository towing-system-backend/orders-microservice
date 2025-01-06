namespace Order.Application
{
    public record UpdateOrderStatusCommand(
        string Id,
        string Status
    );
}