namespace Application.Core
{
    public class OrderCancelledEvent
    {
        public Guid OrderId { get; }
        public DateTime CancelledAt { get; set; }

        public OrderCancelledEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}