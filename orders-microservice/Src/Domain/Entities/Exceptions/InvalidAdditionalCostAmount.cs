using Application.Core;

namespace Order.Domain
{
    public class InvalidAdditionalCostAmountException : DomainException
    {
        public InvalidAdditionalCostAmountException() : base("Invalid additional cost amount, It must be positive.") { }
    }
}
