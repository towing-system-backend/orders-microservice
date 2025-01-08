using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderClientInformationPhoneNumberException() : DomainException("Invalid client phone number.");
}