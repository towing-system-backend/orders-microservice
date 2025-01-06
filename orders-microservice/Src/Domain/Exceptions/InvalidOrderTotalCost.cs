using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderTotalCostException : DomainException
    {
        public InvalidOrderTotalCostException() : base("Invalid order total cost, it must be a positive number.") { }
    }
}
