namespace Application.Core
{
    public class OrderCreatedEventt
    {
        public Guid OrderId { get; }
        public DateTime? CreatedAt { get; set; }
        public OrderCreatedEventt(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}