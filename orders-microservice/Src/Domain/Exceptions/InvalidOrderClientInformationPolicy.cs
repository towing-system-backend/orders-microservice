using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderClientInformationPolicyException : DomainException
{
    public InvalidOrderClientInformationPolicyException() : base("Invalid client policy id"){}
}