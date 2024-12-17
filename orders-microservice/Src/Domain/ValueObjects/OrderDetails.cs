using Application.Core;
using orders_microservice.Domain.Exceptions;

namespace orders_microservice.Domain.ValueObjects;

public class OrderDetails : IValueObject<OrderDetails>
{
    private readonly string _Value;

    public OrderDetails(string value)
    {
        if (value.Length < 8)
        {
            throw new InvalidOrderDetailsException();
        }
        
        _Value = value;
    }
    
    public string GetValue() => _Value;

    public bool Equals(OrderDetails other) => _Value == other._Value;

}