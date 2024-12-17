using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidOrderClientInformationPhoneNumberException : DomainException
{
    public InvalidOrderClientInformationPhoneNumberException() : base("Invalid client phone number"){}    
}