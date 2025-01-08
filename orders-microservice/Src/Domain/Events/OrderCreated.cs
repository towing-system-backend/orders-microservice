using Application.Core;

namespace Order.Domain
{
    public class OrderCreatedEvent(string publisherId, string type, OrderCreated context) : DomainEvent(publisherId, type, context) { }
    public class OrderCreated(
        string status,
        string issueLocation,
        string destination,
        string issuer,
        string details,
        string name,
        string image,
        string policyId,
        string phoneNumber,
        int identificationNumber,
        decimal totalCost
        )
    {
        public readonly string Status = status;
        public readonly string IssueLocation = issueLocation;
        public readonly string Destination = destination;
        public readonly string Issuer = issuer;
        public readonly string Details = details;
        public readonly string Name = name;
        public readonly string Image = image;
        public readonly string PolicyId = policyId;
        public readonly string PhoneNumber = phoneNumber;
        public readonly int IdentificationNumber = identificationNumber;
        public readonly decimal TotalCost = totalCost;
        public readonly List<AdditionalCost>? AdditionalCosts = new();

        public static OrderCreatedEvent CreateEvent(
            OrderId publisherId,
            OrderStatus status,
            OrderIssueLocation issueLocation,
            OrderDestinationLocation destination,
            OrderIssuer issuer,
            OrderDetails details,
            OrderClientInformation clientInformation,
            OrderTotalCost totalCost
        )
        {
            return new OrderCreatedEvent(
                publisherId.GetValue(),
                typeof(OrderCreated).Name,
                new OrderCreated(
                    status.GetValue(),
                    issueLocation.GetValue(),
                    destination.GetValue(),
                    issuer.GetValue(),
                    details.GetValue(),
                    clientInformation.GetClientName(),
                    clientInformation.GetClientImage(),
                    clientInformation.GetClientPolicyId(),
                    clientInformation.GetClientPhoneNumber(),
                    clientInformation.GetClientIdentificationNumber(),
                    totalCost.GetValue()
                )
            );
        }
    }
}