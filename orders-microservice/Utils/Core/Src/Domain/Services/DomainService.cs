namespace Application.Core
{
    public interface IDomainService<T, U>
    {
        Task<Result<U>> Execute(T data);
    }
}