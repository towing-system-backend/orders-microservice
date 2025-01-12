namespace RabbitMQ.Contracts 
{
    public record EventOrderLocated
    (
        Guid OrderId,
        DateTime UpdatedAt
    ) : IRabbitMQMessage;
}

