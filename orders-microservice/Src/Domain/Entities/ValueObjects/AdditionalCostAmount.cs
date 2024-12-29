using Application.Core;
using orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects
{
    public class AdditionalCostAmount : IValueObject<AdditionalCostAmount>
    {
        private readonly decimal _Value;

        public AdditionalCostAmount(decimal value)
        {
            if(value < 0)
            {
                throw new InvalidAdditionalCostAmountException();
            }

            _Value = value;
        }

        public decimal GetValue() => _Value;

        public bool Equals(AdditionalCostAmount other) => _Value == other._Value;
    }
}
