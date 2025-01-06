using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderStatusException : DomainException
    {
        public InvalidOrderStatusException() : base("Invalid order status.") { }
    }
}