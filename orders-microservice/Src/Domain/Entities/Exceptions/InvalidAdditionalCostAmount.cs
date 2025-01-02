using Application.Core;

namespace orders_microservice.Src.Domain.Entities.AdditionalCost.Exceptions
{
    public class InvalidAdditionalCostAmountException : DomainException
    {
        public InvalidAdditionalCostAmountException() : base("Invalid additional cost amount, It must be positive") { }
       
    }
}
