using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderException : DomainException
    {
        public InvalidOrderException() : base("Invalid order, one or more value object are null") { }
    }
}