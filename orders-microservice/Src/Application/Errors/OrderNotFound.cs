using Application.Core;

namespace orders_microservice.Application.Errors;

public class OrderNotFoundError : ApplicationError
{
    public OrderNotFoundError() : base("No order was found") { }
}