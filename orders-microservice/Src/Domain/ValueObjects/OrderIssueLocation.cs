using Application.Core;

namespace Order.Domain
{
    public class OrderIssueLocation : IValueObject<OrderIssueLocation>
    {
        private readonly string _value;

        public OrderIssueLocation(string value)
        {
            if (value is not string)
            {
                throw new InvalidOrderIssueLocationException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(OrderIssueLocation other) => _value == other._value;
    }
}