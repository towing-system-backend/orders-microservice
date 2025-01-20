using Application.Core;
using MongoDB.Driver;
using Order.Application;

namespace Order.Infrastructure
{
    public class FindOrderByIdQuery
    {
        private readonly IMongoCollection<MongoOrder> _orderCollection;
        public FindOrderByIdQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _orderCollection = database.GetCollection<MongoOrder>("orders");
        }
        public async Task<Result<FindOrderByIdResponse>> Execute(FindOrderByIdDto query)
        {
            var filter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, query.Id);
            var res = await _orderCollection.Find(filter).FirstOrDefaultAsync();

            if (res == null) throw new OrderNotFoundError();
            
            var order = new FindOrderByIdResponse(
                res.OrderId,
                res.Status,
                res.IssueLocation,
                res.Destination,
                res.Issuer,
                res.TowDriverAssigned,
                res.Details,
                res.Name,
                res.Image,
                res.PolicyId,
                res.PhoneNumber,
                res.TotalCost,
                res.TotalDistance,
                res.IdentificationNumber,
                res.AdditionalCosts.Select(cost => new AdditonalCostResponse(
                    cost.AdditionalCostId,
                    cost.Name,
                    cost.Category,
                    cost.Amount
                )).ToList()
            );

            return Result<FindOrderByIdResponse>.MakeSuccess(order);
        }
    }
}