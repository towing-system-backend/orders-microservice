namespace orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.Events;

public class OrderCreatedEvent
{
    public Guid OrderId { get; }
    public DateTime? CreatedAt { get; set; }
    public OrderCreatedEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}



