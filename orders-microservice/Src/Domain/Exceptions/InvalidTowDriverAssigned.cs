using Application.Core;

namespace Order.Domain
{
    public class InvalidTowDriverAssignedException() : DomainException("Invalid TowDriver Assigned.");
}