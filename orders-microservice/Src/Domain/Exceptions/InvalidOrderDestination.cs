using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderDestinationException : DomainException
{
    public InvalidOrderDestinationException() : base("Invalid order destination") { }
}