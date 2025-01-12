using System.Text;
using System.Text.Json.Nodes;
using RestSharp;

namespace Application.Core
{
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
            var originTask = FindCoordinates(origin);
            var towLocationTasks = towAddresses.Select(async tow => 
            {
                var coordinates = await FindCoordinates(tow.Value);
                return new JsonObject
                {
                    ["TowDriverId"] = tow.Key,
                    ["Latitude"] = coordinates["lat"]?.GetValue<double>(),
                    ["Longitude"] = coordinates["lng"]?.GetValue<double>(),
                    ["Address"] = tow.Value
                };
            });
            
            var originCoordinates = await originTask;
            var towLocations = new JsonArray(await Task.WhenAll(towLocationTasks));
            
            var destinationsBuilder = new StringBuilder();
            foreach (var location in towLocations.AsArray())
            {
                if (destinationsBuilder.Length > 0)
                    destinationsBuilder.Append('|');
                destinationsBuilder.Append(location["Latitude"]).Append(',').Append(location["Longitude"]);
            }
            
            var originLatLng = $"{originCoordinates["lat"]},{originCoordinates["lng"]}";
            
            var request = new RestRequest("maps/api/distancematrix/json", Method.Get)
                .AddParameter("origins", originLatLng)
                .AddParameter("destinations", destinationsBuilder.ToString())
                .AddParameter("key", _apiKey);

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful) 
                throw new HttpRequestException($"Error retrieving distances: {response.StatusCode}");

            var distanceMatrixJson = JsonNode.Parse(response.Content!);
            var elements = distanceMatrixJson?["rows"]?[0]?["elements"];

            if (elements == null)
                throw new InvalidOperationException("Invalid response format from distance matrix API");
            var locations = new List<JsonNode>(towLocations);
            for (var i = 0; i < elements.AsArray().Count; i++)
            {
                var element = elements[i];
                locations[i]!["Distance"] = element?["distance"]?["value"]?.GetValue<double>();
                locations[i]!["EstimatedTimeOfArrival"] = element?["duration"]?["text"]?.ToString();
            }
            
            return new JsonObject
            {
                ["Origin"] = originLatLng,
                ["TowLocations"] = new JsonArray(
                    locations.OrderBy(loc => loc!["Distance"]?.GetValue<double>())
                        .Select(loc => loc!.DeepClone()) 
                        .ToArray()
                )
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
                ["Steps"] = JsonNode.Parse(route["steps"]?.ToJsonString() ?? "[]") // Clonar el nodo steps
            };

            return shortestRoute;
        }
        
    }
}