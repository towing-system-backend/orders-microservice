using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderClientInformationImageException : DomainException
    {
        public InvalidOrderClientInformationImageException() : base("Invalid client image url.") { }
    }
}