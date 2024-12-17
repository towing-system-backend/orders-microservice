using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderIssueLocationException : DomainException
{
    public InvalidOrderIssueLocationException() : base("Invalid order issue location."){}
}
