using Application.Core;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions
{
    public class InvalidAdditionalCostCategoryException : DomainException
    {
        public InvalidAdditionalCostCategoryException() : base("Invalid additional cost category, It must not be empty")
        {
        }
    }
}
