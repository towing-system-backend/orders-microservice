using Application.Core;

namespace Order.Domain
{
    public class AdditionalCostName : IValueObject<AdditionalCostName>
    {
        private readonly string _value;

        public AdditionalCostName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidAdditionalCostNameException();
            }
            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(AdditionalCostName other) => _value == other._value;
    }
}
