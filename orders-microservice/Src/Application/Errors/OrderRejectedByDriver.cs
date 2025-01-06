using Application.Core;

namespace Order.Application
{
    public class OrderRejectedByDriverError : ApplicationError
    {
        public OrderRejectedByDriverError() : base("Order was rejected by driver.") { }
    }
}
