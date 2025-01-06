using Application.Core;

namespace Order.Domain
{
    public class InvalidTowDriverAssignedException : DomainException
    {
        public InvalidTowDriverAssignedException() : base("Invalid TowDriver Assigned.") { }
    }
}