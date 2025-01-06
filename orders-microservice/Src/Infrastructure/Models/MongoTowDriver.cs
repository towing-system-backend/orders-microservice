using MongoDB.Bson.Serialization.Attributes;

namespace Order.Infrastructure
{
    public class MongoTowDriver
    (
        string towDriverId,
        string name,
        string email,
        string drivingLicenseOwnerName,
        DateOnly drivingLicenseIssueDate,
        DateOnly drivingLicenseExpirationDate,
        string medicalCertificateOwnerName,
        int medicalCertificateOwnerAge,
        DateOnly medicalCertificateIssueDate,
        DateOnly medicalCertificateExpirationDate,
        int identificationNumber,
        string? location,
        string? status
    )
    {
        [BsonId]
        public string TowDriverId = towDriverId;
        public string Name = name;
        public string Email = email;
        public string DrivingLicenseOwnerName = drivingLicenseOwnerName;
        public DateOnly DrivingLicenseIssueDate = drivingLicenseIssueDate;
        public DateOnly DrivingLicenseExpirationDate = drivingLicenseExpirationDate;
        public string MedicalCertificateOwnerName = medicalCertificateOwnerName;
        public int MedicalCertificateOwnerAge = medicalCertificateOwnerAge;
        public DateOnly MedicalCertificateIssueDate = medicalCertificateIssueDate;
        public DateOnly MedicalCertificateExpirationDate = medicalCertificateExpirationDate;
        public int IdentificationNumber = identificationNumber;
        public string? Location = location;
        public string? Status = status;
    }
}
