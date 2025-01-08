using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderException() : DomainException("Invalid order, one or more value object are null");
}