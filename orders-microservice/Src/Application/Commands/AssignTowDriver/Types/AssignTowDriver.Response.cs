namespace orders_microservice.Src.Application.Commands.AssignTowDriver.Types
{
    public record AssignTowDriverResponse
    (
        string TowDriverId,
        double Latitude,
        double Longitude,
        double Distance,
        string Address,
        string EstimatedTimeOfArrival
    );
}
