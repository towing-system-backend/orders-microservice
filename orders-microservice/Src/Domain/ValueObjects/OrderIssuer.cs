using Application.Core;

namespace Order.Domain;

public class OrderIssuer : IValueObject<OrderIssuer>
{
    private readonly string _value;

    public OrderIssuer(string value)
    {
        if (!GuidEx.IsGuid(value!))
        {
            throw new InvalidOrderIssuerIdException();
        }

        _value = value;
    }

    public string GetValue() => _value;
    public bool Equals(OrderIssuer other) => _value == other._value;
}