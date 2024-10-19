namespace Application.Core
{
    public interface IService<T, U>
    {
        Task<Result<U>> Execute(T data);
    }
}