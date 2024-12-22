using orders_microservice.Utils.Core.Src.Domain.Events;

namespace Application.Core 
{ 
    public interface IMessageBrokerService
    {
        Task Publish(List<DomainEvent> domainEvents);
    }
}
