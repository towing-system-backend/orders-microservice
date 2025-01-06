namespace Application.Core
{
    public abstract class DomainEvent(string publisherId, string type, Object context)
    {
        public readonly string PublisherId = publisherId;
        public readonly string Type = type;
        public readonly Object Context = context;
        public readonly DateTime OcurredDate = DateTime.Now;
    }
}