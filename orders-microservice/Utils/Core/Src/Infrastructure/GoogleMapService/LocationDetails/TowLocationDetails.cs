namespace orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService.LocationDetails;

public record TowLocationDetails
{
    public string TowId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Distance { get; set; }
    public string Address { get; set; }
    public string EstimatedTimeOfArrival { get; set; }
}