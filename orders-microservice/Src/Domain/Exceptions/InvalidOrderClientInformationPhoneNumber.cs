using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderClientInformationPhoneNumberException : DomainException
    {
        public InvalidOrderClientInformationPhoneNumberException() : base("Invalid client phone number.") { }
    }
}