using Application.Core;
using orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects
{
    public class AdditionalCostName : IValueObject<AdditionalCostName>
    {
        public string _Value { get; }

        public AdditionalCostName(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new InvalidAdditionalCostNameException();
            }
            _Value = value;
        }

        public string GetValue() => _Value;

        public bool Equals(AdditionalCostName other) => _Value == other._Value;
    }
}
