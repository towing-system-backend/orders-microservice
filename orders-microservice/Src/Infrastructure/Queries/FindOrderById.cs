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
        public async Task<Result<FindOrderByIdResponse>> Execute(FindOrderAssignedDto query)
        {
            var filter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, query.Id);
            var res = await _orderCollection.Find(filter).FirstOrDefaultAsync();

            if (res == null) throw new OrderNotFoundError();
            if (res.TowDriverAssigned == "Not Assigned") throw new NotAssignedTowDriverError();

            var order = new FindOrderByIdResponse
            (
                res.Status,
                res.IssueLocation,
                res.Destination,
                res.Details,
                res.Name,
                res.Image,
                res.PhoneNumber,
                res.TowDriverAssigned
            );

            return Result<FindOrderByIdResponse>.MakeSuccess(order);
        }
    }
}