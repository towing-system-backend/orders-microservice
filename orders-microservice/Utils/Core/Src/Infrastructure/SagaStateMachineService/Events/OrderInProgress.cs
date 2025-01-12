namespace RabbitMQ.Contracts 
{
    public record EventOrderInProgress
    (
        Guid OrderId,
        DateTime UpdatedAt
    ) : IRabbitMQMessage;
}

