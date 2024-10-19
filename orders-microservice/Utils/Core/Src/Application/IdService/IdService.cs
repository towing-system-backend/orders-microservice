namespace Application.Core 
{
    public interface IdService<T> {
        T GenerateId();
    }
}