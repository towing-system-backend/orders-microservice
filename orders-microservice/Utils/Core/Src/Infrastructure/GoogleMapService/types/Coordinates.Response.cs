namespace orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService.types;

public record CoordinatesResponse
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    string? Address { get; init; }
};