namespace Application.Core
{
    public class UpdateOrderStatusEvent
    {
        public Guid OrderId { get; }
        public DateTime? UpdatedAt { get; set; }

        public UpdateOrderStatusEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}