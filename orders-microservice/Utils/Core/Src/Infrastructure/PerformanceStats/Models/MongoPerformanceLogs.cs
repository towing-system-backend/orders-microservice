namespace Application.Core
{
    public class MongoPerformanceLogs(string serviceExecuted, string operationType, long latencyTime)
    {
        public string ServiceExecuted = serviceExecuted;
        public string OperationType = operationType;
        public long LatencyTime = latencyTime;
        public DateTime CreatedAt = DateTime.Now;
    }
}
