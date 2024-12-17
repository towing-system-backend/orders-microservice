namespace RabbitMQ.Contracts
{
    public record EventType(
        string PublisherId,
        string Type,
        object Context,
        DateTime OcurredDate
    );
}
