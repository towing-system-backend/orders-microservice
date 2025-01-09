using Application.Core;

namespace Order.Domain
{
    public class OrderStatus : IValueObject<OrderStatus>
    {
        private readonly string _value;
        private static readonly string[] ValidStatuses =
            ["ToAssign", "ToAccept", "Accepted", "Located", "InProgress", "Completed", "Cancelled", "Paid"];

        public OrderStatus(string value)
        {
            if (!IsValidStatus(value))
            {
                throw new InvalidOrderStatusException();
            }
            _value = value;
        }

        private static bool IsValidStatus(string value)
        {
            return ValidStatuses.Contains(value, StringComparer.OrdinalIgnoreCase);
        }

        public string GetValue() => _value;
        public bool Equals(OrderStatus other) => _value == other._value;
    }
}