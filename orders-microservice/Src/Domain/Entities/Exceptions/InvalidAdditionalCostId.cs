using Application.Core;

namespace Order.Domain
{
    public class InvalidAdditionalCostIdException : DomainException
    {
        public InvalidAdditionalCostIdException() : base("Invalid Additional Cost Id.") { }
    }
}
