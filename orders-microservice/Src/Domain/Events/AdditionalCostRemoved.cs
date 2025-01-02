using orders_microservice.Domain.ValueObjects;
using orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects;
using orders_microservice.Utils.Core.Src.Domain.Events;

namespace orders_microservice.Src.Domain.Events
{
    public class AdditionalCostRemovedEvent(string pusblisherId, string type, AdditionalCostRemoved context) 
        : DomainEvent(pusblisherId, type, context) { }

    public class AdditionalCostRemoved(string Id)
    {
        public readonly string Id = Id;

        public static AdditionalCostRemovedEvent CreateEvent(OrderId publisherId, AdditionalCostId Id)
        {
            return new AdditionalCostRemovedEvent(
                publisherId.GetValue(),
                typeof(AdditionalCostRemoved).Name,
                new AdditionalCostRemoved(
                    Id.GetValue()
                )
            );
        }
    }
}
