namespace RabbitMQ.Contracts 
{
    public record EventOrderCompleted
    (
        Guid OrderId,
        DateTime UpdatedAt 
    ) : IRabbitMQMessage;
}
