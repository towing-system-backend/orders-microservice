using Application.Core;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Order.Infrastructure;

namespace Order.Infrastructure
{
    public class FindTowDriverByStatusQuery
    {
        private readonly IMongoCollection<MongoTowDriver> _towDriverCollection;
        public FindTowDriverByStatusQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _towDriverCollection = database.GetCollection<MongoTowDriver>("tow-drivers");
        }

        public async Task<Result<List<FindTowDriverInformationResponse>>> Execute()
        {
            var filter = Builders<MongoTowDriver>.Filter.Eq(towDriver => towDriver.Status, "Active");
            var res = await _towDriverCollection.Find(filter).ToListAsync();

            if (res.IsNullOrEmpty())
                return Result<List<FindTowDriverInformationResponse>>.MakeError(new Exception("Tow drivers not found."));

            var towDriversInfo = res.Select(
                t => new FindTowDriverInformationResponse
                (
                    t.TowDriverId,
                    t.Location!,
                    t.Email
                )).ToList();
            foreach(var driver in towDriversInfo)
            {
                Console.WriteLine(driver.Location);
            }
            return Result<List<FindTowDriverInformationResponse>>.MakeSuccess(towDriversInfo);
        }
    }
}