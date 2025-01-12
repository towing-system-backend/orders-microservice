namespace RabbitMQ.Contracts
{
    public record EventOrderAccepted
    (
        Guid OrderId,
        DateTime UpdatedAt
    ) : IRabbitMQMessage;
}
