using Application.Core;
using orders_microservice.Src.Domain.Exceptions;

namespace orders_microservice.Src.Domain.ValueObjects
{
    public class OrderTotalCost : IValueObject<OrderTotalCost>
    {
        private readonly decimal _Value;

        public OrderTotalCost(decimal value)
        {
            if (value < 0)
            {
                throw new InvalidOrderTotalCostException();
            }
            _Value = value;
        }
        public decimal GetValue() => _Value;
        public bool Equals(OrderTotalCost other)
        {
            throw new NotImplementedException();
        }
    }
}
