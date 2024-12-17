using Application.Core;
using orders_microservice.Domain.Exceptions;
namespace orders_microservice.Domain.ValueObjects;

public class OrderId : IValueObject<OrderId>
{
    private readonly string _Value;

    public OrderId(string value)
    {
        if (!GuidEx.IsGuid(value))
        {
            throw new InvalidOrderIdException();
        }
        _Value = value;
    }
    
    public string GetValue() => _Value;

    public bool Equals(OrderId other) => _Value == other._Value;
}