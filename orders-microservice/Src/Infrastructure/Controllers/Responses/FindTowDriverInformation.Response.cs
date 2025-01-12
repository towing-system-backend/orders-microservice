namespace Order.Infrastructure
{
    public record FindTowDriverInformationResponse
    (
        string TowDriverId,
        string Location,
        string? Email
    );
}
