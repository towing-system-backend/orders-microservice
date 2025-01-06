using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderDestinationException : DomainException
    {
        public InvalidOrderDestinationException() : base("Invalid order destination.") { }
    }
}