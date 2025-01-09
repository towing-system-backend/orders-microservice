using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderClientInformationNameException() : DomainException("Invalid client name.");
}