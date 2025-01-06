using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderIdException : DomainException
    {
        public InvalidOrderIdException() : base("Invalid order id.") { }
    }
}