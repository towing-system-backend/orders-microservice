using Application.Core;
namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderClientInformationNameException : DomainException
{
    public InvalidOrderClientInformationNameException() : base("Invalid client name."){ }
}