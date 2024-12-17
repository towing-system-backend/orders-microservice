using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderException : DomainException
{
    public InvalidOrderException() : base("Invalid order, one or more value object are null"){ }
}