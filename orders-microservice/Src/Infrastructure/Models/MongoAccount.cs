using MongoDB.Bson.Serialization.Attributes;

namespace Order.Infrastructure
{
    public class MongoAccount(string userId, string supplierCompanyId, string deviceId, string email, string role, string password, DateTime? passwordExpirationDate)
    {
        [BsonId]
        public string UserId = userId;
        public string SupplierCompanyId = supplierCompanyId;
        public string? DeviceId = deviceId;
        public string Email = email;
        public string Role = role;
        public string Password = password;
        public DateTime? PasswordExpirationDate = passwordExpirationDate;
        public DateTime CreatedAt = DateTime.Now;
    }
}
