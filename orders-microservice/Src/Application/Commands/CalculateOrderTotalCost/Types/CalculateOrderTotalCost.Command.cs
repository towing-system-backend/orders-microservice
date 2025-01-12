namespace Order.Application
{
    public record CalculateOrderTotalCostCommand
    (
        string OrderId,
        int CoverageAmount,
        int CoverageDistance
    );
}
