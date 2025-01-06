using MongoDB.Driver;

namespace Application.Core
{
    public class SagaStateMachineRepository : ISagaStateMachineService<String>
    {
        private readonly IMongoCollection<MongoStates> _statesCollection;

        public SagaStateMachineRepository() 
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _statesCollection = database.GetCollection<MongoStates>("status-events");
        }

        public async Task<List<string>> FindRejectedDrivers(string orderId)
        {
            var filter = Builders<MongoStates>.Filter.Eq(state => state.CorrelationId, Guid.Parse(orderId));
            var sagaState = await _statesCollection.Find(filter).FirstOrDefaultAsync();
            return sagaState.DriversThatRejected ?? new List<string>();
        }
    }
}
