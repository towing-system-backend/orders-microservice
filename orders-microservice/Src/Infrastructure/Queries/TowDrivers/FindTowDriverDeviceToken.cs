using Application.Core;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Order.Infrastructure;


namespace Order.Infrastructure
{
    public class FindTowDriversDeviceTokenQuery
    {
        private readonly IMongoCollection<MongoAccount> _accountsCollection;
        public FindTowDriversDeviceTokenQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_JOSE"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_JOSE"));
            _accountsCollection = database.GetCollection<MongoAccount>("accounts");
        }

        public async Task<Result<Dictionary<string, string>>> Execute(List<FindTowDriverInformationResponse> drivers) 
        {
            drivers.ForEach(driver => Console.WriteLine($"TowDriverId: {driver.TowDriverId}, Email: {driver.Email}"));

            var emails = drivers.Select(d => d.Email).ToList();
            Console.WriteLine(string.Join(", ", emails));

            var filter = Builders<MongoAccount>.Filter.In(account => account.Email, emails);

            var projection = Builders<MongoAccount>.Projection
                .Include(account => account.Email)
                .Include(account => account.DeviceId);

            var res = await _accountsCollection
                .Find(filter)
                .Project<MongoAccount>(projection)
                .ToListAsync();
            
            if (res.IsNullOrEmpty())
            {
                return Result<Dictionary<string, string>>.MakeError(new Exception("Devices id not found"));
            }

            var devicesToken = drivers
                .Where(driver => !string.IsNullOrEmpty(driver.Email))
                .Join(
                    res,
                    driver => driver.Email,
                    account => account.Email,
                    (driver, account) => new { driver.TowDriverId, account.DeviceId }
                )
                .ToDictionary(
                    pair => pair.TowDriverId,
                    pair => pair.DeviceId
                );
            return Result<Dictionary<string, string>>.MakeSuccess(devicesToken);
        }
    }
}
