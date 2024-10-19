namespace Application.Core
{
    public interface IEventStore
    {
        public Task AppendEvents(List<DomainEvent> events);
    }
}