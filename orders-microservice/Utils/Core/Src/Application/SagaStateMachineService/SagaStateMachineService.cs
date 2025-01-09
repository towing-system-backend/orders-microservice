namespace Application.Core
{
    public interface ISagaStateMachineService<T>
    {
       Task<List<T>> FindRejectedDrivers(string OrderId);
    }
}
