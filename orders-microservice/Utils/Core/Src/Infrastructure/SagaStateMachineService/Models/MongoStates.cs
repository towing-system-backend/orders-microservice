using MongoDB.Bson.Serialization.Attributes;

namespace Application.Core
{
    public class MongoStates
    (
        Guid id,
        string currentState,
        DateTime createdAt, 
        DateTime? lastChange,
        string driverThatAccept,
        string deviceToken,
        List<string> driversThatRejected,
        int version
    )
    {
        [BsonId]
        public Guid CorrelationId = id;
        public string CurrentState = currentState;
        public DateTime CreatedAt = createdAt;
        public DateTime? LastStateChange = lastChange;
        public string? DriverThatAccept = driverThatAccept;
        public string? DeviceToken = deviceToken;
        public List<string> DriversThatRejected = driversThatRejected;
        public int Version = version;
    }
}
