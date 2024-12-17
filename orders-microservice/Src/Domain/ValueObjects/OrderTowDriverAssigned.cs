using Application.Core;
using orders_microservice.Domain.Exceptions;

namespace orders_microservice.Domain.ValueObjects;

public class OrderTowDriverAssigned : IValueObject<OrderTowDriverAssigned>
{
    private readonly string? _Value;

    public OrderTowDriverAssigned(string? value)
    {
        if (value is null && !GuidEx.IsGuid(value))
        {
            throw new InvalidTowDriverAssignedException();
        }

        _Value = value;
    }
    
    public string GetValue() => _Value;

    public bool Equals(OrderTowDriverAssigned other) => _Value == other._Value;

}