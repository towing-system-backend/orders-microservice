namespace Application.Core
{
    public class EventOrderRejected
    {
        public Guid OrderId { get; }
        public string? TowDriverId { get; set; }
        public DateTime CancelledAt { get; set; }

        public EventOrderRejected(Guid orderId, string towDriverId)
        {
            OrderId = orderId;
            TowDriverId = towDriverId;
        }
    }
}