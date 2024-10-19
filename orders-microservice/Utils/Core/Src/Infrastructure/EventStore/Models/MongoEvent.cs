using MongoDB.Bson.Serialization.Attributes;

namespace Application.Core
{
    public class MongoEvent
    {
        [BsonId]
        public string Stream { get; set; } = null!;
        public string Type { get; set; } = null!;
        public Object Data { get; set; } = null!;
        public string OcurredDate { get; set; } = null!;
    }
}

