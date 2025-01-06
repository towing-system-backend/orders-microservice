using Application.Core;

namespace Order.Domain
{
    public class AdditionalCostAmount : IValueObject<AdditionalCostAmount>
    {
        private readonly decimal _value;

        public AdditionalCostAmount(decimal value)
        {
            if (value < 0)
            {
                throw new InvalidAdditionalCostAmountException();
            }

            _value = value;
        }

        public decimal GetValue() => _value;
        public bool Equals(AdditionalCostAmount other) => _value == other._value;
    }
}
