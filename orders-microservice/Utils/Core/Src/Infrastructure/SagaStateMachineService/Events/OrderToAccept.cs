namespace RabbitMQ.Contracts
{
    public record EventOrderToAccept
    (
        Guid OrderId,
        string? TowDriverId,
        string? DeviceToken,
        DateTime UpdatedAt
    ) : IRabbitMQMessage;
};

