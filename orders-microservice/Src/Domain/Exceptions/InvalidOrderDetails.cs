using Application.Core;
namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderDetailsException : DomainException
{
    public InvalidOrderDetailsException() : base("Details must be specified."){}
}