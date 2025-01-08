using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderTotalCostException()
        : DomainException("Invalid order total cost, it must be a positive number.");
}
