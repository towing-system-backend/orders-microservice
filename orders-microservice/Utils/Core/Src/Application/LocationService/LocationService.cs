namespace orders_microservice.Utils.Core.Src.Application.LocationService;

public interface ILocationService<T>
{
    Task<T> FindCoordinates(string location);

    Task<T> FindNearestTow(Dictionary<string,string> towAdress, string origin);

    Task<T> FindShortestRoute(string origin, string destination);
}
