using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderIdException : DomainException
{
    public InvalidOrderIdException() : base("Invalid order id"){}
}