using orders_microservice.Utils.Core.Src.Application.MapService;
using RestSharp;

namespace orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService;

public class GoogleMapService : ILocationService<string>
{
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("MAPS_API_KEY");
    
    public async Task<string> FindCoordinates(string location)
    {
        var client = new RestClient("https://maps.googleapis.com");
        var request = new RestRequest("maps/api/geocode/json", Method.Get);

        request.AddParameter("address", location);
        request.AddParameter("Key", _apiKey);
        
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful) 
            throw new HttpRequestException("Error trying to resolve the request from google maps.");
        return response.Content;
    }

}