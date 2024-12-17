using MongoDB.Driver;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;
using orders_microservice.Infrastructure.Controllers;
using orders_microservice.Infrastructure.Controllers.Dtos;
using IOptional = Application.Core.Optional<Order>;
using IOptionals = Application.Core.Optional<System.Collections.Generic.List<orders_microservice.Infrastructure.Controllers.Dtos.FindOrderByStatusDto>>;

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
                .Set(order => order.PhoneNumber, order.GetOrderClientInformation.GetClientPhoneNumber());

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
                    true
                )
            );
        }
    }
}