namespace Application.Core
{
    public class EventOrderCancelled
    {
        public Guid OrderId { get; }
        public DateTime CancelledAt { get; set; }

        public EventOrderCancelled(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}