using Application.Core;

namespace Order.Domain
{
    public class AdditionalCostId : IValueObject<AdditionalCostId>
    {
        private readonly string _value;

        public AdditionalCostId(string value)
        {
            if (!GuidEx.IsGuid(value))
            {
                throw new InvalidAdditionalCostIdException();
            }
            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(AdditionalCostId other) => _value == other._value;
    }
}