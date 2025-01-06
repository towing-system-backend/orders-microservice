using Application.Core;

namespace Order.Domain
{
    public class InvalidAdditionalCostNameException : DomainException
    {
        public InvalidAdditionalCostNameException() : base("Invalid additional cost name.") { }
    }
}