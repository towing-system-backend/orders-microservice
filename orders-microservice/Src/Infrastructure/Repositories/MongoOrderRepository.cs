using MongoDB.Driver;
using Order.Domain;

using IOptional = Application.Core.Optional<Order.Domain.Order>;

namespace Order.Infrastructure
{
    public class MongoOrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<MongoOrder> _orderCollection;
        public MongoOrderRepository()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _orderCollection = database.GetCollection<MongoOrder>("orders");
        }

        public async Task Save(Domain.Order order)
        {

            var filter = Builders<MongoOrder>.Filter.Eq(MongoOrder => MongoOrder.OrderId, order.GetOrderId().GetValue());
            var update = Builders<MongoOrder>.Update
                .Set(mongoOrder => mongoOrder.Status, order.GetOrderStatus().GetValue())
                .Set(mongoOrder => mongoOrder.IssueLocation, order.GetOrderIssueLocation().GetValue())
                .Set(mongoOrder => mongoOrder.Destination, order.GetOrderDestinationLocation().GetValue())
                .Set(mongoOrder => mongoOrder.Issuer, order.GetOrderIssuer().GetValue())
                .Set(mongoOrder => mongoOrder.TowDriverAssigned, order.GetOrderTowDriverAssigned()?.GetValue() ?? "Not assigned")
                .Set(mongoOrder => mongoOrder.Details, order.GetOrderDetails().GetValue())
                .Set(mongoOrder => mongoOrder.Name, order.GetOrderClientInformation().GetClientName())
                .Set(mongoOrder => mongoOrder.Image, order.GetOrderClientInformation().GetClientImage())
                .Set(mongoOrder => mongoOrder.PolicyId, order.GetOrderClientInformation().GetClientPolicyId())
                .Set(mongoOrder => mongoOrder.PhoneNumber, order.GetOrderClientInformation().GetClientPhoneNumber())
                .Set(mongoOrder => mongoOrder.IdentificationNumber, order.GetOrderClientInformation().GetClientIdentificationNumber())
                .Set(mongoOrder => mongoOrder.TotalCost, order.GetOrderTotalCost().GetValue())
                .Set(mongoOrder => mongoOrder.TotalDistance, order.GetOrderTotalDistance().GetValue())
                .Set(mongoOrder => mongoOrder.AdditionalCosts, order.GetAdditionalCosts()?.Select(cost => new MongoAdditionalCost(
                        cost.GetAdditionalCostId().GetValue(),
                        cost.GetAdditionalCostName().GetValue(),
                        cost.GetAdditionalCostCategory().GetValue(),
                        cost.GetAdditionalCostAmount().GetValue()
                )).ToList());

            await _orderCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
    
        public async Task<IOptional> FindById(string orderId)
        {
            var filter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, orderId);
            var res = await _orderCollection.Find(filter).FirstOrDefaultAsync();
        
            if (res == null) return IOptional.Empty();

            return IOptional.Of(
                Domain.Order.Create(new OrderId(res.OrderId),
                    new OrderStatus(res.Status),
                    new OrderIssueLocation(res.IssueLocation),
                    new OrderDestinationLocation(res.Destination),
                    new OrderTowDriverAssigned(res.TowDriverAssigned),
                    new OrderIssuer(res.Issuer),
                    new OrderDetails(res.Details),
                    new OrderClientInformation
                        (res.Name, res.Image, res.PolicyId, res.PhoneNumber, res.IdentificationNumber),
                    new OrderTotalCost(res.TotalCost),
                    new OrderTotalDistance(res.TotalDistance),
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