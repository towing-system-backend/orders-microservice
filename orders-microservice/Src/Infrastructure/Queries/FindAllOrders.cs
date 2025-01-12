using Application.Core;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Order.Application;

namespace Order.Infrastructure
{
    public class FindAllOrdersQuery 
    {
        private readonly IMongoCollection<MongoOrder> _orderCollection;
        public FindAllOrdersQuery()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _orderCollection = database.GetCollection<MongoOrder>("orders");
        }
        public async Task<Result<List<FindAllOrdersResponse>>> Execute()
        {
            var filter = Builders<MongoOrder>.Filter.Empty;
            var res = await _orderCollection.Find(filter).ToListAsync();
            if (res.IsNullOrEmpty()) 
                throw new OrdersNotFoundError();
            var orders = res.Select(order => new FindAllOrdersResponse
                (
                    order.OrderId,
                    order.Status,
                    order.IssueLocation,
                    order.Destination,
                    order.TowDriverAssigned,
                    order.Details,
                    order.Name,
                    order.Image,
                    order.PolicyId,
                    order.PhoneNumber,
                    order.TotalCost,
                    order.AdditionalCosts!.Select(cost => new AdditonalCostResponse
                    (
                        cost.AdditionalCostId,
                        cost.Name,
                        cost.Category,
                        cost.Amount
                    )).ToList()
                ))
                .ToList();
            return Result<List<FindAllOrdersResponse>>.MakeSuccess(orders);
        }
    }
}