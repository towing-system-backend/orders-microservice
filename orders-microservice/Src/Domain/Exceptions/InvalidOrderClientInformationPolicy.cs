using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderClientInformationPolicyException : DomainException
    {
        public InvalidOrderClientInformationPolicyException() : base("Invalid client policy id.") { }
    }
}