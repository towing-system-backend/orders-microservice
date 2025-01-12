namespace Application.Core
{
    public class EventOrderCreated
    {
        public Guid OrderId { get; }
        public DateTime? CreatedAt { get; set; }
        public EventOrderCreated(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}