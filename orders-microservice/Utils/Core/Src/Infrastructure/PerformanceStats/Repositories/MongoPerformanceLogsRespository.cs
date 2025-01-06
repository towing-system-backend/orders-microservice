using MongoDB.Driver;

namespace Application.Core
{
    public interface IPerformanceLogsRepository
    {
        public void LogStats(string serviceExecuted, string operationType, long latencyTime);
    }

    public class MongoPerformanceLogsRespository : IPerformanceLogsRepository
    {
        private readonly IMongoCollection<MongoPerformanceLogs> _performanceLogsCollection;
        public MongoPerformanceLogsRespository()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _performanceLogsCollection = database.GetCollection<MongoPerformanceLogs>("performance-logs");
        }
        public async void LogStats(string serviceExecuted, string operationType, long latencyTime)
        {
            var perfomanceLogs = new MongoPerformanceLogs(serviceExecuted, operationType, latencyTime);

            await _performanceLogsCollection.InsertOneAsync(perfomanceLogs);
        }
    }
}
