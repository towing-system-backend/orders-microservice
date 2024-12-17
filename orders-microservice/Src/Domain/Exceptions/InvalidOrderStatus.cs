using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderStatusException : DomainException
{
    public InvalidOrderStatusException() : base("Invalid order status"){}
}