using MongoDB.Driver;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;
using orders_microservice.Infrastructure.Controllers;
using orders_microservice.Src.Domain.Entities.AdditionalCost;
using orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects;
using orders_microservice.Src.Domain.ValueObjects;
using System.Linq;
using IOptional = Application.Core.Optional<Order>;

namespace orders_microservice.Infrastructure.Repositories
{
    public class MongoOrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<MongoOrder> _orderCollection;
        public MongoOrderRepository()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _orderCollection = database.GetCollection<MongoOrder>("orders");
        }
        public async Task Save(Order order)
        {
            Console.WriteLine($"{order.GetAdditionalCosts}");
            var filter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, order.GetOrderId.GetValue());
            var update = Builders<MongoOrder>.Update
                .Set(order => order.Status, order.GetOrderStatus.GetValue())
                .Set(order => order.IssueLocation, order.GetOrderIssueLocation.GetValue())
                .Set(order => order.Destination, order.GetOrderDestinationLocation.GetValue())
                .Set(order => order.TowDriverAssigned, order.GetOrderTowDriverAssigned?.GetValue() ?? "Not assigned")
                .Set(order => order.Details, order.GetOrderDetails.GetValue())
                .Set(order => order.Name, order.GetOrderClientInformation.GetClientName())
                .Set(order => order.Image, order.GetOrderClientInformation.GetClientImage())
                .Set(order => order.PolicyId, order.GetOrderClientInformation.GetClientPolicyId())
                .Set(order => order.PhoneNumber, order.GetOrderClientInformation.GetClientPhoneNumber())
                .Set(order => order.TotalCost, order.GetOrderTotalCost.GetValue())
                .Set(order => order.AdditionalCosts, order.GetAdditionalCosts?.Select(cost => new MongoAdditionalCost(
                        cost.GetAdditionalCostId.GetValue(),
                        cost.GetAdditionalCostName.GetValue(),
                        cost.GetAdditionalCostCategory.GetValue(),
                        cost.GetAdditionalCostAmount.GetValue()
                )).ToList());

            await _orderCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
    
        public async Task<IOptional> FindById(string orderId)
        {
            var filter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, orderId);
            var res = await _orderCollection.Find(filter).FirstOrDefaultAsync();
        
            if (res == null) return IOptional.Empty();

            return IOptional.Of(
                Order.Create(new OrderId(res.OrderId),
                    new OrderStatus(res.Status),
                    new OrderIssueLocation(res.IssueLocation),
                    new OrderDestinationLocation(res.Destination),
                    new OrderTowDriverAssigned(res.TowDriverAssigned),
                    new OrderDetails(res.Details),
                    new OrderClientInformation( res.Name, res.Image, res.PolicyId, res.PhoneNumber),
                    new OrderTotalCost(res.TotalCost),
                    res.AdditionalCosts?.Select(ac => new AdditionalCost(
                        new AdditionalCostId(ac.AdditionalCostId),
                        new AdditionalCostAmount(ac.Amount),
                        new AdditionalCostName(ac.Name),
                        new AdditionalCostCategory(ac.Category)
                    )).ToList(),
                    true
                )
            );
        }
    }
}