using GoogleMapsApi;
using GoogleMapsApi.Entities.Geocoding.Request;
using orders_microservice.Utils.Core.Src.Application.MapService;
using orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService.types;

namespace orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService;

public class GoogleMapService : ILocationService<CoordinatesResponse>
{
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("MAPS_API_KEY");
    
    public async Task<CoordinatesResponse> FindCoordinates(string location)
    {
        var geocodeRequest = new GeocodingRequest { Address = location, ApiKey = _apiKey };
        var geocodingEngine = GoogleMaps.Geocode;
        var geocodeResponse = await geocodingEngine.QueryAsync(geocodeRequest);
        var firstResult = geocodeResponse.Results.First();
        var coordinates = new CoordinatesResponse
        {
            Latitude = firstResult.Geometry.Location.Latitude,
            Longitude = firstResult.Geometry.Location.Longitude
        };
        return coordinates;
    }

}