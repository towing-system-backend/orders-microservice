using MongoDB.Bson.Serialization.Attributes;

namespace Application.Core
{
    public class MongoStates
    (
        Guid id,
        string currentState,
        DateTime createdAt, 
        DateTime? lastChange,
        List<string> driversThatRejected,
        int version
    )
    {
        [BsonId]
        public Guid CorrelationId = id;
        public string CurrentState = currentState;
        public DateTime CreatedAt = createdAt;
        public DateTime? LastStateChange = lastChange;
        public List<string> DriversThatRejected = driversThatRejected;
        public int Version = version;
    }
}
