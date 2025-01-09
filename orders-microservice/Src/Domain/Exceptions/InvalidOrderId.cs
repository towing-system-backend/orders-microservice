using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderIdException() : DomainException("Invalid order id.");
}