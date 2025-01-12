using Application.Core;

namespace orders_microservice.Src.Domain.Exceptions
{
    public class InvalidOrderTotalDistanceException() : DomainException("Invalid distance, must be positive") { }
}
