using System.Text.Json;
using System.Text.Json.Nodes;
using orders_microservice.Utils.Core.Src.Application.LocationService;
using orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService.LocationDetails;
using RestSharp;

namespace orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService;

public class GoogleMapService : ILocationService<TowLocationDetails>
{
    private readonly string? _client = Environment.GetEnvironmentVariable("API_CLIENT_URI")!;
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("MAPS_API_KEY")!;
    
    public async Task<TowLocationDetails> FindCoordinates(string location)
    {
        var client = new RestClient(_client!);
        var request = new RestRequest("maps/api/geocode/json", Method.Get)
            .AddParameter("address", location)
            .AddParameter("key", _apiKey);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful) throw new HttpRequestException($"Request failed: {response.StatusCode} - {response.StatusDescription}");
        var json = JsonNode.Parse(response.Content ?? string.Empty);
        var status = json?["status"]?.ToString();
        if (status != "OK") throw new Exception($"Google Maps API returned an error: {status}");
        var locationNode = json?["results"]?[0]?["geometry"]?["location"];
        return new TowLocationDetails
        {
            Latitude = locationNode?["lat"]?.GetValue<double>() ?? 0,
            Longitude = locationNode?["lng"]?.GetValue<double>() ?? 0,
            Address = location
        };
    }
    
    public async Task<List<TowLocationDetails>> FindNearestTow(Dictionary<string, string> towAddresses, string origin)
    {
        var client = new RestClient(_client!);
        var originCoordinates = await FindCoordinates(origin);
        var towLocations = new List<TowLocationDetails>();

        foreach (var tow in towAddresses)
        {
            var coordinates = await FindCoordinates(tow.Value);
            towLocations.Add(new TowLocationDetails
            {
                TowId = tow.Key,
                Latitude = coordinates.Latitude,
                Longitude = coordinates.Longitude,
                Address = tow.Value
            });
        }

        var destinations = string.Join("|", towLocations.Select(c => $"{c.Latitude},{c.Longitude}"));
        var originLatLng = $"{originCoordinates.Latitude},{originCoordinates.Longitude}";

        var request = new RestRequest("maps/api/distancematrix/json", Method.Get)
            .AddParameter("origins", originLatLng)
            .AddParameter("destinations", destinations)
            .AddParameter("key", _apiKey);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful) throw new Exception("Error while retrieving distances.");

        var json = JsonDocument.Parse(response.Content!);
        var elements = json.RootElement.GetProperty("rows")[0].GetProperty("elements");

        for (var i = 0; i < elements.GetArrayLength(); i++)
        {
            towLocations[i] = towLocations[i] with
            {
                Distance = elements[i].GetProperty("distance").GetProperty("value").GetDouble(),
                EstimatedTimeOfArrival = elements[i].GetProperty("duration").GetProperty("text").GetString()
            };
        }

        return towLocations.OrderBy(t => t.Distance).ToList();
    }
    public async Task<TowLocationDetails> FindShortestRoute(string origin, string destination)
    {
        var client = new RestClient(_client!);
        var request = new RestRequest("maps/api/directions/json", Method.Get)
            .AddParameter("origin", origin)
            .AddParameter("destination", destination)
            .AddParameter("key", _apiKey);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful) throw new Exception("Error retrieving route.");

        var json = JsonDocument.Parse(response.Content!);
        var route = json.RootElement.GetProperty("routes")[0]
            .GetProperty("legs")[0];

        var distance = route.GetProperty("distance").GetProperty("value").GetDouble();
        var duration = route.GetProperty("duration").GetProperty("text").GetString();

        return new TowLocationDetails
        {
            Distance = distance,
            Address = destination,
            EstimatedTimeOfArrival = duration 
        };
    }
}