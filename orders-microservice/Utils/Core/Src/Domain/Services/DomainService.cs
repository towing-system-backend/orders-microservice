namespace Application.Core
{
    public interface IDomainService<T, U>
    {
        U Execute(T data);
    }
}