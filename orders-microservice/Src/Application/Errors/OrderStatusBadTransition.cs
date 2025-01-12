using Application.Core;

namespace Order.Application
{
    public class OrderStatusBadTransitionError(string status)
        : ApplicationError($"Status can not be in that status from {status}"){ }
}
