using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderClientInformationImageException : DomainException
{
    public InvalidOrderClientInformationImageException() : base("Invalid client image url."){}
}