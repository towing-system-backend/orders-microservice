using MongoDB.Bson.Serialization.Attributes;

namespace Order.Infrastructure
{
    public class MongoDepartment(
        string departmentId,
        string name,
        List<string> employees
    )
    {
        [BsonId]
        public string DepartmentId = departmentId;
        public string Name = name;
        public List<string> Employees = employees;
    }

    public class MongoPolicy(
        string policyId,
        string title,
        int coverageAmount,
        int coverageDistance,
        decimal price,
        string type,
        DateOnly issuanceDate,
        DateOnly expirationDate
    )
    {
        [BsonId]
        public string PolicyId = policyId;
        public string Title = title;
        public int CoverageAmount = coverageAmount;
        public int CoverageDistance = coverageDistance;
        public decimal Price = price;
        public string Type = type;
        public DateOnly IssuanceDate = issuanceDate;
        public DateOnly ExpirationDate = expirationDate;
    }

    public class MongoSupplierCompany(string supplierCompanyId,
        List<MongoDepartment> departments,
        List<MongoPolicy> policies,
        List<string> towDrivers,
        string name,
        string phoneNumber,
        string type,
        string rif,
        string state,
        string city,
        string street
    )
    {
        [BsonId]
        public string SupplierCompanyId = supplierCompanyId;
        public List<MongoDepartment> Departments = departments;
        public List<MongoPolicy> Policies = policies;
        public List<string> TowDrivers = towDrivers;
        public string Name = name;
        public string PhoneNumber = phoneNumber;
        public string Type = type;
        public string Rif = rif;
        public string State = state;
        public string City = city;
        public string Street = street;
        public DateTime CreatedAt = DateTime.Now;
    }
}
