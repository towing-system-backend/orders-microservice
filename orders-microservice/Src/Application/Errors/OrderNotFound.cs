namespace orders_microservice.Application.Errors;

public class OrderNotFoundError : ApplicationException
{
    public OrderNotFoundError() : base("No order was found") { }
}