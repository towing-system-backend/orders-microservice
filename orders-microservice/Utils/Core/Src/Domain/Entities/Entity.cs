namespace Application.Core
{
    public abstract class Entity<T>(T id) where T : IValueObject<T>
    {
        protected T Id { get; private set; } = id;

        public bool Equals(Entity<T> other)
        {
            return Id.Equals(other.Id);
        }
    }
}