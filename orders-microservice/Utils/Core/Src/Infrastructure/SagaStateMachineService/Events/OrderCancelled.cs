namespace RabbitMQ.Contracts
{
    public record EventOrderCancelled
    (
        Guid OrderId, 
        DateTime CancelledAt
    ) : IRabbitMQMessage;
}