using Application.Core;

namespace orders_microservice.Domain.Events;
using orders_microservice.Domain.ValueObjects;



public class OrderDestinationLocationUpdatedEvent(string publisherId, string type, OrderDestinationLocationUpdated context) : DomainEvent(publisherId, type, context) { }

public class OrderDestinationLocationUpdated(string destinationLocation)
{
    public readonly string DestinationLocation = destinationLocation;

    public static OrderDestinationLocationUpdatedEvent CreateEvent(OrderId publisherId, OrderDestinationLocation destinationLocation)
    {
        return new OrderDestinationLocationUpdatedEvent(
            publisherId.GetValue(),
            typeof(OrderDestinationLocationUpdated).Name,
            new OrderDestinationLocationUpdated(
                destinationLocation.GetValue()
            )
        );
    }
}