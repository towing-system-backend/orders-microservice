using Application.Core;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Order.Infrastructure
{
    public class FindTowDriverByStatusQuery
    {
        private readonly IMongoCollection<MongoTowDriver> _towDriverCollection;
        public FindTowDriverByStatusQuery()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _towDriverCollection = database.GetCollection<MongoTowDriver>("tow-drivers");
        }

        public async Task<Result<Dictionary<string, string>>> Execute()
        {
            var filter = Builders<MongoTowDriver>.Filter.Eq(towDriver => towDriver.Status, "Active");
            var res = await _towDriverCollection.Find(filter).ToListAsync();

            if (res.IsNullOrEmpty())
                return Result<Dictionary<string, string>>.MakeError(new Exception("Tow drivers not found."));

            var towDriversLocation = res.ToDictionary(towDriver => towDriver.TowDriverId, towDriver => towDriver.Location!);

            return Result<Dictionary<string, string>>.MakeSuccess(towDriversLocation);
        }
    }
}