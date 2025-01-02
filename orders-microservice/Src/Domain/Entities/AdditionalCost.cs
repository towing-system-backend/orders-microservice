using Application.Core;
using orders_microservice.Domain.ValueObjects;
using orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions;
using orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects;
using orders_microservice.Utils.Core.Src.Domain.Entities;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost
{
    public class AdditionalCost
    (
        AdditionalCostId id,
        AdditionalCostAmount amount,
        AdditionalCostName name,
        AdditionalCostCategory category
    ) : Entity<AdditionalCostId>(id)
    {
        protected AdditionalCostId Id = id;
        protected AdditionalCostAmount Amount =  amount;
        protected AdditionalCostName Name = name;
        protected AdditionalCostCategory Category = category;
        
        public bool Equals(AdditionalCost other)
        {
            return Id.Equals(other.Id);
        }
        public AdditionalCostId GetAdditionalCostId => Id;
        public AdditionalCostAmount GetAdditionalCostAmount => Amount;
        public AdditionalCostName GetAdditionalCostName => Name;
        public AdditionalCostCategory GetAdditionalCostCategory => Category;
    }
}
