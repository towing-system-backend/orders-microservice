using Application.Core;
using MongoDB.Bson;
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
            
            var policyFilter = Builders<MongoSupplierCompany>.Filter
                .ElemMatch(company => company.Policies, policy => policy.PolicyId == order.PolicyId);


            var projection = Builders<MongoSupplierCompany>.Projection
                .Include("Policies.$");  
            
            var result = await _supplierCompanyCollection
                .Find(policyFilter)
                .Project(projection)
                .FirstOrDefaultAsync();
            
            var policyArray = result?["Policies"]?.AsBsonArray;
            var policy = policyArray?.FirstOrDefault()?.AsBsonDocument;

            if (policy == null)
                throw new InvalidOperationException("Policy is not found");

            return Result<ClientPolicyResponse>.MakeSuccess(
                new ClientPolicyResponse(
                    policy["CoverageAmount"].AsInt32,
                    policy["CoverageDistance"].AsInt32
                )
            );
        }
    }
}
