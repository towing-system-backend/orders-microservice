using Application.Core;

namespace Order.Domain
{
    public class AdditionalCost
    (
        AdditionalCostId id,
        AdditionalCostAmount amount,
        AdditionalCostName name,
        AdditionalCostCategory category
    ) : Entity<AdditionalCostId>(id)
    {
        private readonly AdditionalCostId _id = id;
        private readonly AdditionalCostAmount _amount =  amount;
        private readonly AdditionalCostName _name = name;
        private readonly AdditionalCostCategory _category = category;
        
        public bool Equals(AdditionalCost other) => _id.Equals(other._id);
        public AdditionalCostId GetAdditionalCostId() => _id;
        public AdditionalCostAmount GetAdditionalCostAmount() => _amount;
        public AdditionalCostName GetAdditionalCostName() => _name;
        public AdditionalCostCategory GetAdditionalCostCategory() => _category;
    }
}