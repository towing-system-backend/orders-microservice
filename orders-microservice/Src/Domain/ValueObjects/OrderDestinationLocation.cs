using Application.Core;
using orders_microservice.Domain.Exceptions;

namespace orders_microservice.Domain.ValueObjects;

public class OrderDestinationLocation : IValueObject<OrderDestinationLocation>
{
    private readonly string _Value;

    public OrderDestinationLocation(string value)
    {
        if (value is not string)
        {
            throw new InvalidOrderDestinationException();
        }
        _Value = value;
    }
    
    public string GetValue() => _Value;

    public bool Equals(OrderDestinationLocation other) => _Value == other._Value;
}