using Application.Core;

namespace Order.Domain
{
    public class OrderDestinationLocation : IValueObject<OrderDestinationLocation>
    {
        private readonly string _value;

        public OrderDestinationLocation(string value)
        {
            if (value is not string)
            {
                throw new InvalidOrderDestinationException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(OrderDestinationLocation other) => _value == other._value;
    }
}