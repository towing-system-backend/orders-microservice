using Application.Core;
using MongoDB.Driver;
using Order.Application;

namespace Order.Infrastructure
{
    public class FindOrderByStatusQuery 
    {
        private readonly IMongoCollection<MongoOrder> _orderCollection;
        public FindOrderByStatusQuery()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _orderCollection = database.GetCollection<MongoOrder>("orders");
        }
        public async Task<Result<List<FindOrderByStatusResponse>>> Execute(FindOrderByStatusDto query)
        {
            var filter = Builders<MongoOrder>.Filter.Eq(order => order.Status, query.Status);
            var orders = await _orderCollection.Find(filter)
                .Project(order => new FindOrderByStatusResponse
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
                    order.AdditionalCosts.Select(cost => new AdditonalCostResponse
                    (
                        cost.AdditionalCostId,
                        cost.Name,
                        cost.Category,
                        cost.Amount
                    )).ToList()
                ))
                .ToListAsync();

            if (!orders.Any()) throw new OrdersNotFoundError();
            return Result<List<FindOrderByStatusResponse>>.MakeSuccess(orders);
        }
    }
}