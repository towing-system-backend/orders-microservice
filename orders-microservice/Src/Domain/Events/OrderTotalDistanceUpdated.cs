using Application.Core;
using Order.Domain;

namespace orders_microservice.Src.Domain.Events
{
    public class OrderTotalDistanceUpdatedEvent(string publisherId, string type, OrderTotalDistanceUpdated context)
        : DomainEvent(publisherId, type, context)
    { }
    public class OrderTotalDistanceUpdated(double totalDistance)
    {
        public readonly double TotalDistance = totalDistance;
        public static OrderTotalDistanceUpdatedEvent CreateEvent(OrderId publisherId, OrderTotalDistance totalDistance)
        {
            return new OrderTotalDistanceUpdatedEvent(
                publisherId.GetValue(),
                nameof(OrderTotalDistanceUpdated),
                new OrderTotalDistanceUpdated(totalDistance.GetValue())
            );
        }
    }
}
