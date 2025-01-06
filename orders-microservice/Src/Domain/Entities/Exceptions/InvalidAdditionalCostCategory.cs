using Application.Core;

namespace Order.Domain
{
    public class InvalidAdditionalCostCategoryException : DomainException
    {
        public InvalidAdditionalCostCategoryException() : base("Invalid additional cost category, It must not be empty.") { }
    }
}
