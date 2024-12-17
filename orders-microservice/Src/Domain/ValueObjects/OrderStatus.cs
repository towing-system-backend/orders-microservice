using Application.Core;
using orders_microservice.Domain.Exceptions;

namespace orders_microservice.Domain.ValueObjects;

public class OrderStatus : IValueObject<OrderStatus>
{
    private readonly string _Value;

    public OrderStatus(String value)
    {
        if(value is not string)
        {
            throw new InvalidOrderStatusException();
        }
        _Value = value;
    }
    
    public string GetValue() => _Value;

    public bool Equals(OrderStatus other) => _Value == other._Value;
}