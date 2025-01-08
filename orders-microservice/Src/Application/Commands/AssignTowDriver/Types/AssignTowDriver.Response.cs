namespace Order.Application
{
    public record AssignTowDriverResponse
    (
        string TowDriverId,
        double Latitude,
        double Longitude,
        double Distance,
        string Address,
        string? EstimatedTimeOfArrival
    );
}
