using orders_microservice.Utils.Core.Src.Domain.Events;

namespace Application.Core
{
    public interface IEventStore
    {
        public Task AppendEvents(List<DomainEvent> events);
    }
}