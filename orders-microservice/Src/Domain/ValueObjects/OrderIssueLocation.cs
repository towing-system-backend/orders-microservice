using Application.Core;
using orders_microservice.Domain.Exceptions;

namespace orders_microservice.Domain.ValueObjects;

public class OrderIssueLocation : IValueObject<OrderIssueLocation>
{
    private readonly string _Value;

    public OrderIssueLocation(string value)
    {
        if (value is not string)
        {
            throw new InvalidOrderIssueLocationException();
        }
        _Value = value;
    }
    
    public string GetValue() => _Value;

    public bool Equals(OrderIssueLocation other) => _Value == other._Value;
}