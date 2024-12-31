namespace orders_microservice.Utils.Core.Src.Application.SagaStateMachineService
{
    public interface ISagaStateMachineService<T>
    {
       Task<List<T>> FindRejectedDrivers(string OrderId);
    }
}
