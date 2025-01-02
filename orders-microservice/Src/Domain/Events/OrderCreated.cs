using orders_microservice.Domain.ValueObjects;
using orders_microservice.Src.Domain.Entities.AdditionalCost;
using orders_microservice.Src.Domain.ValueObjects;
using orders_microservice.Utils.Core.Src.Domain.Events;

namespace orders_microservice.Domain.Events;

public class OrderCreatedEvent(string publisherId, string type, OrderCreated context) : DomainEvent(publisherId, type, context) { }

public class OrderCreated(
    string status,
    string issueLocation,
    string destination,
    string details, 
    string name,
    string image,
    string policyId,
    string phoneNumber,
    decimal totalCost,
    List<AdditionalCost>? additionalCosts
    )
{
    public readonly string Status = status;
    public readonly string IssueLocation = issueLocation;
    public readonly string Destination = destination;
    public readonly string Details = details;
    public readonly string Name = name;
    public readonly string Image = image;
    public readonly string PolicyId = policyId;
    public readonly string PhoneNumber = phoneNumber;
    public readonly decimal TotalCost = totalCost;
    public readonly List<AdditionalCost>? AdditionalCosts = new();

    public static OrderCreatedEvent CreateEvent(
        OrderId publisherId,
        OrderStatus status,
        OrderIssueLocation issueLocation,
        OrderDestinationLocation destination,
        OrderDetails details,
        OrderClientInformation clientInformation,
        OrderTotalCost totalCost,
        List<AdditionalCost> additionalCosts
    )
    {
        return new OrderCreatedEvent(
            publisherId.GetValue(),
            typeof(OrderCreated).Name,
            new OrderCreated(
                status.GetValue(),
                issueLocation.GetValue(),
                destination.GetValue(),
                details.GetValue(),
                clientInformation.GetClientName(),
                clientInformation.GetClientImage(),
                clientInformation.GetClientPolicyId(),
                clientInformation.GetClientPhoneNumber(),
                totalCost.GetValue(),
                null
            )
        );
    }
}