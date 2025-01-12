namespace RabbitMQ.Contracts 
{
    public record EventOrderPaid
    (
        Guid OrderId,
        DateTime UpdatedAt
    ) : IRabbitMQMessage;
};

