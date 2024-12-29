using Application.Core;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions
{
    public class InvalidAdditionalCostIdException : DomainException
    {
        public InvalidAdditionalCostIdException() : base("Invalid Additional Cost Id"){}
        
    }
}
