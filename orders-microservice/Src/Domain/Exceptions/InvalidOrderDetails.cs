using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderDetailsException() : DomainException("Details must be specified.");
}