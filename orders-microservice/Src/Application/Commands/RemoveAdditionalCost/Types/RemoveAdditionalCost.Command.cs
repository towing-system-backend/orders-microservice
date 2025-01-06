namespace Order.Application
{
    public record RemoveAdditionalCostCommand(
        string OrderId,
        string AdditionalCostId
    );
}
