namespace Application.Core
{
    public class EventUpdateOrderStatus
    {
        public Guid OrderId { get; }
        public DateTime? UpdatedAt { get; set; }

        public EventUpdateOrderStatus(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}