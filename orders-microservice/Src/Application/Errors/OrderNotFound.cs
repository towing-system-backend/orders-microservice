using Application.Core;

namespace Order.Application
{
    public class OrderNotFoundError : ApplicationError
    {
        public OrderNotFoundError() : base("No order was found.") { }
    }
}