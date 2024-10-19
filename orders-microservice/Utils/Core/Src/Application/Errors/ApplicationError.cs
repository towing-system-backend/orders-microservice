namespace Application.Core
{
    public abstract class ApplicationError(string message) : Exception(message) { }
}