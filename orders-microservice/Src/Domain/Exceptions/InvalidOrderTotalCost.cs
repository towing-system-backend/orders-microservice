using Application.Core;

namespace orders_microservice.Src.Domain.Exceptions
{
    public class InvalidOrderTotalCostException : DomainException
    {
        public InvalidOrderTotalCostException() : base("Invalid order total cost, it must be a positive number")
        {
        }
    }
}
