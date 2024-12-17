namespace Application.Core 
{ 
    public interface IMessageBrokerService
    {
        Task Publish(List<DomainEvent> domainEvents);
    }
}
