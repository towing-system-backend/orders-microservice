namespace orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.Events;

public class UpdateOrderStatusEvent
{
    public Guid OrderId { get; }
    public DateTime? UpdatedAt { get; set; }

    public UpdateOrderStatusEvent(Guid orderId) 
    {
        OrderId = orderId;
    }
}