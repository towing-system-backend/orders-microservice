using Application.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Order.Application;
using Order.Infrastructure;

namespace Order.Infrastructure
{
    public class FindClientPolicyQuery
    {
        private readonly IMongoCollection<MongoSupplierCompany> _supplierCompanyCollection;
        private readonly IMongoCollection<MongoOrder> _orderCollection;

        public FindClientPolicyQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _supplierCompanyCollection = database.GetCollection<MongoSupplierCompany>("supplier-companies");
            _orderCollection = database.GetCollection<MongoOrder>("orders");
        }

        public async Task<Result<ClientPolicyResponse>> Execute(string OrderId)
        {

            var orderFilter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, OrderId);
            var order = await _orderCollection.Find(orderFilter).FirstOrDefaultAsync();
            if (order == null)
                return Result<ClientPolicyResponse>.MakeError(new OrderNotFoundError());

            var companyFilter = Builders<MongoSupplierCompany>.Filter
                .ElemMatch(x => x.Policies, policy => policy.PolicyId == order.PolicyId);

            var projection = Builders<MongoSupplierCompany>.Projection
                .Include("Policies.$");

            var result = await _supplierCompanyCollection
                .Find(companyFilter)
                .Project(projection)
                .FirstOrDefaultAsync();

            var policy = result["Policies"].AsBsonArray[0].AsBsonDocument;

            return Result<ClientPolicyResponse>.MakeSuccess(
                new ClientPolicyResponse(policy["CoverageAmount"].AsInt32, policy["CoverageDistance"].AsInt32));
        }
    }
}
