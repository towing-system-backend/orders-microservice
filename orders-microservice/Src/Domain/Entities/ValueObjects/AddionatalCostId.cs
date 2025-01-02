
using Application.Core;
using orders_microservice.Domain.Exceptions;
using orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects
{
    public class AdditionalCostId : IValueObject<AdditionalCostId>
    {
        private readonly string _Value;

        public AdditionalCostId(string value)
        {
            if (!GuidEx.IsGuid(value))
            {
                throw new InvalidAdditionalCostIdException();
            }
            _Value = value;
        }
        public string GetValue() => _Value;

        public bool Equals(AdditionalCostId other) => _Value == other._Value;
    }
}
