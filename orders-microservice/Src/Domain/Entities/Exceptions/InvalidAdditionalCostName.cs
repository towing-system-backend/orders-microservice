using Application.Core;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions
{
    public class InvalidAdditionalCostNameException : DomainException
    {
        public InvalidAdditionalCostNameException() : base("Invalid additional cost name"){}
    }
}
