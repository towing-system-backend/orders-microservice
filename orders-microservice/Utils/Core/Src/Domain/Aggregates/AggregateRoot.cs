using System.Reflection;

namespace Application.Core
{
    public abstract class AggregateRoot<T>(T id) : Entity<T>(id) where T : IValueObject<T>
    {
        protected List<DomainEvent> Events = [];

        protected new T Id { get; private set; } = id;

        public List<DomainEvent> PullEvents()
        {
            var events = Events;
            Events = [];
            return events;
        } 

        public void Apply(DomainEvent @event)
        {
            var eventHandler = GetEventHandler(@event.GetType().Name) ?? throw new Exception("Event handler not found.");
            eventHandler.Invoke(this, new object[] {@event.Context});
            ValidateState();
            Events.Add(@event);
        }

        public MethodInfo? GetEventHandler(string eventHandler) 
        {
            MethodInfo? method = GetType().GetMethod($"On{eventHandler}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return method;
        }

        public abstract void ValidateState();
    }
}
