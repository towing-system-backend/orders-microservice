namespace Application.Core
{
    public interface IValueObject<T>
    {
        bool Equals(T other);
    }
}