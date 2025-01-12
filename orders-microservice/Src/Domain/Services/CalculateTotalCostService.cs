using Application.Core;

namespace Order.Domain.Services 
{
    public class CalculateTotalCostService: IDomainService<List<AdditionalCost>, OrderTotalCost>
    {
        public OrderTotalCost Execute(List<AdditionalCost> additionalCosts)
        {
            decimal totalCost = 0;
            foreach (var cost in additionalCosts) totalCost += cost.GetAdditionalCostAmount().GetValue();
            return new OrderTotalCost(totalCost);
        }
    }
}

