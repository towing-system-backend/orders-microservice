namespace orders_microservice.Utils.Core.Src.Application.MapService;

public interface ILocationService<T>
{
    Task<T> FindCoordinates(string location);
}