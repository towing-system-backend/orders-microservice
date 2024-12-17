using Application.Core;

namespace orders_microservice.Application.Errors;

public class NotAssignedTowDriverError : ApplicationError
{
    public NotAssignedTowDriverError() : base("this order has not been assigned.") { }
}