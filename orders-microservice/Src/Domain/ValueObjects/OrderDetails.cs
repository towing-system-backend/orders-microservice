using Application.Core;

namespace Order.Domain
{
    public class OrderDetails : IValueObject<OrderDetails>
    {
        private readonly string _value;

        public OrderDetails(string value)
        {
            if (value.Length < 8)
            {
                throw new InvalidOrderDetailsException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(OrderDetails other) => _value == other._value;
    }
}