using Application.Core;
using orders_microservice.Domain.ValueObjects;
using orders_microservice.Utils.Core.Src.Domain.Events;

namespace orders_microservice.Domain.Events;

public class OrderStatusUpdatedEvent(string publisherId, string type, OrderStatusUpdated context) : DomainEvent(publisherId, type, context) { }

public class OrderStatusUpdated(string status)
{
    public readonly string Status = status;

    public static OrderStatusUpdatedEvent CreateEvent(OrderId publisherId, OrderStatus status)
    {
        return new OrderStatusUpdatedEvent(
            publisherId.GetValue(),
            typeof(OrderStatusUpdated).Name,
            new OrderStatusUpdated(
                status.GetValue()
            )
        );
    }
}