using Application.Core;

namespace Order.Domain
{
    public class OrderTowDriverAssignedUpdatedEvent(string publisherId, string type, OrderTowDriverAssignedUpdated context) : DomainEvent(publisherId, type, context) { }

    public class OrderTowDriverAssignedUpdated(string towDriverAssigned)
    {
        public readonly string TowDriverAssigned = towDriverAssigned;

        public static OrderTowDriverAssignedUpdatedEvent CreateEvent(OrderId publisherId, OrderTowDriverAssigned towDriverAssigned)
        {
            return new OrderTowDriverAssignedUpdatedEvent(
                publisherId.GetValue(),
                typeof(OrderTowDriverAssignedUpdated).Name,
                new OrderTowDriverAssignedUpdated(
                    towDriverAssigned.GetValue()
                )
            );
        }
    }
}