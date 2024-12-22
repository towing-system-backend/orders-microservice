using Application.Core;
using orders_microservice.Utils.Core.Src.Domain.Events;

namespace orders_microservice.Domain.Events;
using orders_microservice.Domain.ValueObjects;



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