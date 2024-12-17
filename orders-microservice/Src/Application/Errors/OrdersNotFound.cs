using Application.Core;

namespace orders_microservice.Application.Errors;

public class OrdersNotFoundError : ApplicationError
{
    public OrdersNotFoundError() : base("Any order was not found") { }
}