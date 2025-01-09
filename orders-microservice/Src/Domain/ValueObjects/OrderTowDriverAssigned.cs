using Application.Core;

namespace Order.Domain
{
    public class OrderTowDriverAssigned : IValueObject<OrderTowDriverAssigned>
    {
        private readonly string _value;

        public OrderTowDriverAssigned(string value)
        {
            if (value.Equals("Not Assigned.")  || !GuidEx.IsGuid(value))
            { 
                _value = value; 
            } 
            else if (GuidEx.IsGuid(value))
            {
                _value = value;
            } else 
            {
                throw new InvalidTowDriverAssignedException();
            }
        }

        public string GetValue() => _value;
        public bool Equals(OrderTowDriverAssigned other) => _value == other._value;
    }
}