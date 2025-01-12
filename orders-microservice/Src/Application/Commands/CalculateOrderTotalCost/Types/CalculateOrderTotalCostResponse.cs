namespace Order.Application
{
    public record CalculateOrderTotalCostResponse
    (
        int CoverageAmount,
        decimal AdditionalCosts,
        double? ExcessDistance,
        decimal Debt
    );
}
