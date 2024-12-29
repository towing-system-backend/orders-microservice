using MongoDB.Driver;
using Newtonsoft.Json;


namespace Application.Core
{
    public class MongoEventStore : IEventStore 
    {
        private readonly IMongoCollection<MongoEvent> _eventCollection;

        public MongoEventStore()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _eventCollection = database.GetCollection<MongoEvent>("events");

            var indexKeysDefinition = Builders<MongoEvent>.IndexKeys.Ascending(e => e.Stream);
            var indexModel = new CreateIndexModel<MongoEvent>(indexKeysDefinition);
            _eventCollection.Indexes.CreateOne(indexModel);

        }

        public async Task AppendEvents(List<DomainEvent> events)
        {

            var mappedEvents = events.Select(e => 
                new MongoEvent(
                    e.PublisherId,                  
                    e.Type,
                    JsonConvert.SerializeObject(e.Context, Formatting.Indented), 
                    e.OcurredDate                
                )
            );

            await _eventCollection.InsertManyAsync(mappedEvents);
        }
    }
}
