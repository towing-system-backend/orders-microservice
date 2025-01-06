using Application.Core;

namespace Order.Domain
{
    public class OrderTowDriverAssigned : IValueObject<OrderTowDriverAssigned>
    {
        private readonly string? _value;

        public OrderTowDriverAssigned(string? value)
        {
            if (value is null && !GuidEx.IsGuid(value))
            {
                throw new InvalidTowDriverAssignedException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(OrderTowDriverAssigned other) => _value == other._value;
    }
}