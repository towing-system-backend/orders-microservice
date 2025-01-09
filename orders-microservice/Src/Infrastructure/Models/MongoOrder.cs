using MongoDB.Bson.Serialization.Attributes;

namespace Order.Infrastructure
{
    public class MongoOrder(
        string id,
        string status,
        string issueLocation,
        string destination,
        string issuer,
        string towDriverAssigned,
        string details,
        string name,
        string image,
        string policy,
        string phoneNumber,
        int identificationNumber,
        decimal totalCost,
        List<MongoAdditionalCost> additionalCosts
    )
    {
        [BsonId]
        public string OrderId = id;
        public string Status = status;
        public string IssueLocation = issueLocation;
        public string Destination = destination;
        public string Issuer = issuer;
        public string TowDriverAssigned = towDriverAssigned;
        public string Details = details;
        public string Name = name;
        public string Image = image;
        public string PolicyId = policy;
        public string PhoneNumber = phoneNumber;
        public int IdentificationNumber = identificationNumber;
        public decimal TotalCost = totalCost;
        public List<MongoAdditionalCost>? AdditionalCosts = additionalCosts ?? [];
    }

    public class MongoAdditionalCost(
        string id,
        string name,
        string category,
        decimal amount
    )
    {
        [BsonId]
        public string AdditionalCostId = id;
        public string Name = name;
        public string Category = category;
        public decimal Amount = amount;
    }
}