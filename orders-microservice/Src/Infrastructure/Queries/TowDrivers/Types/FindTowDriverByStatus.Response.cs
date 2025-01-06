namespace Order.Infrastructure
{
    public record FindTowDriverByStatusResponse(
        string TowDriverId,
        string TowDriverName,
        string TowDriverEmail,
        string LicenseOwnerName,
        DateOnly LicenseIssueDate,
        DateOnly LicenseExpirationDate,
        string MedicalCertificateOwnerName,
        int MedicalCertificateAge,
        DateOnly MedicalCertificateIssueDate,
        DateOnly MedicalCertificateExpirationDate,
        int TowDriverIdentificationNumber,
        string Location,
        string Status
    );
}