using Application.Core;
using orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects
{
    public class AdditionalCostCategory : IValueObject<AdditionalCostCategory>
    {
        private readonly string _Value;

        public AdditionalCostCategory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidAdditionalCostCategoryException();
            }
            _Value = value;
        }

        public string GetValue() => _Value;

        public bool Equals(AdditionalCostCategory other) => _Value == other._Value;
    }
}
