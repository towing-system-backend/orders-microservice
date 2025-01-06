namespace Application.Core
{
    public interface ILocationService<T>
    {
        Task<T> FindCoordinates(string location);

        Task<T> FindNearestTow(Dictionary<string, string> towAdress, string origin);

        Task<T> FindShortestRoute(string origin, string destination);
    }
}