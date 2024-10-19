namespace Application.Core
{
    public abstract class DomainException(string message) : Exception(message) { }
}