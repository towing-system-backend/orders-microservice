using Application.Core;

namespace orders_microservice.Src.Application.Errors
{
    public class OrderRejectedByDriverError : ApplicationError
    {
        public OrderRejectedByDriverError() : base("Order was rejected by driver") { }
    }
}
