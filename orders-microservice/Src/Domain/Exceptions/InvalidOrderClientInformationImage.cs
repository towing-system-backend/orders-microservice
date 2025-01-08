using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderClientInformationImageException() : DomainException("Invalid client image url.");
}