using Application.Core;

namespace orders_microservice.Domain.Exceptions;

public class InvalidTowDriverAssignedException : DomainException
{
    public InvalidTowDriverAssignedException() : base("Invalid TowDriver Assigned"){}
}