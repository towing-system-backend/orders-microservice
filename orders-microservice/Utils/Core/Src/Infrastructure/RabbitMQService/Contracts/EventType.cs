namespace RabbitMQ.Contracts
{
    public interface IRabbitMQMessage { };
    public record EventType(
        string PublisherId,
        string Type,
        object Context,
        DateTime OcurredDate
    );

    public record DriverResponse
    (
        string PublisherId,
        string Status
    ) : IRabbitMQMessage;
}