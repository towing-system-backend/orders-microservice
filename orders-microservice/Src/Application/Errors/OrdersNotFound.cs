using Application.Core;

namespace Order.Application
{
    public class OrdersNotFoundError : ApplicationError
    {
        public OrdersNotFoundError() : base("Any order was not found.") { }
    }
}