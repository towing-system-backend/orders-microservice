using Application.Core;

namespace Order.Domain
{
    public class OrderTotalCost : IValueObject<OrderTotalCost>
    {
        private readonly decimal _value;

        public OrderTotalCost(decimal value)
        {
            if (value < 0)
            {
                throw new InvalidOrderTotalCostException();
            }

            _value = value;
        }

        public decimal GetValue() => _value;
        public bool Equals(OrderTotalCost other) => _value == other._value;
    }
}