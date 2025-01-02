using MongoDB.Bson.Serialization.Attributes;

namespace orders_microservice.Infrastructure.Controllers;

public class MongoOrder(
    string id,
    string status,
    string issueLocation,
    string destination,
    string towDriverAssigned,
    string details,
    string name,
    string image,
    string policy,
    string phoneNumber,
    decimal totalCost,
    List<MongoAdditionalCost> additionalCosts
)
{
    [BsonId] 
    public string OrderId = id;
    public string Status = status;
    public string IssueLocation = issueLocation;
    public string Destination = destination;
    public string TowDriverAssigned = towDriverAssigned;
    public string Details = details;
    public string Name = name;
    public string Image = image;
    public string PolicyId = policy;
    public string PhoneNumber = phoneNumber;
    public decimal TotalCost = totalCost;
    public List<MongoAdditionalCost>? AdditionalCosts = additionalCosts ?? new List<MongoAdditionalCost>();
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