using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderDetailsException : DomainException
    {
        public InvalidOrderDetailsException() : base("Details must be specified.") { }
    }
}