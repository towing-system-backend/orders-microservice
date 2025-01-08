using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderClientInformationPolicyException() : DomainException("Invalid client policy id.");
}