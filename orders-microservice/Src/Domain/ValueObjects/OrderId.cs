using Application.Core;

namespace Order.Domain
{
    public class OrderId : IValueObject<OrderId>
    {
        private readonly string _value;

        public OrderId(string value)
        {
            if (!GuidEx.IsGuid(value))
            {
                throw new InvalidOrderIdException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(OrderId other) => _value == other._value;
    }
}