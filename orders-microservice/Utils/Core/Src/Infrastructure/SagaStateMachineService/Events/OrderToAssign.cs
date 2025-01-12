namespace RabbitMQ.Contracts
{
    public record EventOrderToAssign
    (
        Guid OrderId,
        DateTime CreatedAt 
    ) : IRabbitMQMessage;
}