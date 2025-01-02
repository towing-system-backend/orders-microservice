using System.Text.Json;
using System.Text.Json.Nodes;
using orders_microservice.Utils.Core.Src.Application.LocationService;
using RestSharp;

namespace orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService;

public class GoogleMapService : ILocationService<JsonNode>
{
    private readonly string? _client = Environment.GetEnvironmentVariable("API_CLIENT_URI")!;
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("MAPS_API_KEY")!;

    public async Task<JsonNode> FindCoordinates(string location)
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
        return locationNode!;
    }

    public async Task<JsonNode> FindNearestTow(Dictionary<string, string> towAddresses, string origin)
    {
        var client = new RestClient(_client!);
        var originCoordinates = await FindCoordinates(origin);
        var towLocations = new JsonArray();

        foreach (var tow in towAddresses)
        {
            var coordinates = await FindCoordinates(tow.Value);
            var location = new JsonObject
            {
                ["TowDriverId"] = tow.Key,
                ["Latitude"] = coordinates["lat"]?.GetValue<double>(),
                ["Longitude"] = coordinates["lng"]?.GetValue<double>(),
                ["Address"] = tow.Value
            };

            towLocations.Add(location);
        }

        var destinations = string.Join("|", towLocations.Select(c =>
            $"{c["Latitude"]},{c["Longitude"]}"));
        var originLatLng = $"{originCoordinates["lat"]},{originCoordinates["lng"]}";

        var request = new RestRequest("maps/api/distancematrix/json", Method.Get)
            .AddParameter("origins", originLatLng)
            .AddParameter("destinations", destinations)
            .AddParameter("key", _apiKey);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful) throw new Exception("Error while retrieving distances.");

        var distanceMatrixJson = JsonNode.Parse(response.Content!);
        var elements = distanceMatrixJson?["rows"]?[0]?["elements"];

        for (var i = 0; i < elements?.AsArray().Count; i++)
        {
            towLocations[i]["Distance"] = elements[i]?["distance"]?["value"]?.GetValue<double>();
            towLocations[i]["EstimatedTimeOfArrival"] = elements[i]?["duration"]?["text"]?.ToString();
        }

        return new JsonObject
        {
            ["Origin"] = originLatLng,
            ["TowLocations"] = towLocations
        };
    }
    public async Task<JsonNode> FindShortestRoute(string origin, string destination)
    {
        var client = new RestClient(_client!);
        var request = new RestRequest("maps/api/directions/json", Method.Get)
            .AddParameter("origin", origin)
            .AddParameter("destination", destination)
            .AddParameter("key", _apiKey);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful) throw new Exception("Error retrieving route.");

        var json = JsonNode.Parse(response.Content ?? string.Empty);
        var status = json?["status"]?.ToString();

        if (status != "OK") throw new Exception($"Google Maps API returned an error: {status}");
        
        var route = json?["routes"]?[0]?["legs"]?[0];

        if (route == null) throw new Exception("No route information found.");

        var shortestRoute = new JsonObject
        {
            ["Origin"] = origin,
            ["Destination"] = destination,
            ["Distance"] = route["distance"]?["value"]?.GetValue<double>(),
            ["Duration"] = route["duration"]?["text"]?.ToString(), 
            ["StartAddress"] = route["start_address"]?.ToString(),
            ["EndAddress"] = route["end_address"]?.ToString(),
            ["Steps"] = route["steps"] 
        };

        return shortestRoute;
    }
}