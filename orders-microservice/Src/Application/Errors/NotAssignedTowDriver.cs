using Application.Core;

namespace Order.Application
{
    public class NotAssignedTowDriverError : ApplicationError
    {
        public NotAssignedTowDriverError() : base("this order has not been assigned.") { }
    }
}