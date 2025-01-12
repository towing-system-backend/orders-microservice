using Application.Core;
using orders_microservice.Src.Domain.Exceptions;

namespace Order.Domain
{
    public class OrderTotalDistance : IValueObject<OrderTotalDistance>
    {
        private readonly double _value;

        public OrderTotalDistance(double value)
        {
            if(_value < 0)
            {
                throw new InvalidOrderTotalDistanceException();
            }
            _value = value;
        }

        public double GetValue() => _value; 
        public bool Equals(OrderTotalDistance other) => _value == other._value;
    }
}
