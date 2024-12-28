namespace orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.Events;

public class OrderCancelledEvent
{
    public Guid OrderId { get; }
    public DateTime CancelledAt { get; set; }

    public OrderCancelledEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}