using MongoDB.Bson;
using MongoDB.Driver;
using orders_microservice.Utils.Core.Src.Domain.Events;

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
                    e.Context.ToBsonDocument().ToString(), 
                    e.OcurredDate                
                )
            );

            await _eventCollection.InsertManyAsync(mappedEvents);
        }
    }
}
