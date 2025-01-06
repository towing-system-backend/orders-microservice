namespace Application.Core
{
    public class OrderRejectedEvent
    {
        public Guid OrderId { get; }
        public string? TowDriverId { get; set; }
        public DateTime CancelledAt { get; set; }

        public OrderRejectedEvent(Guid orderId, string towDriverId)
        {
            OrderId = orderId;
            TowDriverId = towDriverId;
        }
    }
}