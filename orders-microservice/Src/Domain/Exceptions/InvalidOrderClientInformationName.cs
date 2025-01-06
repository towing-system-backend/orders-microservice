using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderClientInformationNameException : DomainException
    {
        public InvalidOrderClientInformationNameException() : base("Invalid client name.") { }
    }
}