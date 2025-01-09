using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderDestinationException() : DomainException("Invalid order destination.");
}