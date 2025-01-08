using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderStatusException() : DomainException("Invalid order status.");
}