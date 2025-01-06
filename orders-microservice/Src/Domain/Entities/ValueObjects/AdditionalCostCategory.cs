using Application.Core;

namespace Order.Domain
{
    public class AdditionalCostCategory : IValueObject<AdditionalCostCategory>
    {
        private readonly string _value;

        public AdditionalCostCategory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidAdditionalCostCategoryException();
            }
            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(AdditionalCostCategory other) => _value == other._value;
    }
}
