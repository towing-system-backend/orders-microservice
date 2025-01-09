using System.ComponentModel.DataAnnotations;

namespace Order.Infrastructure
{
    public record CreateOrderDto
    (

        [Required][RegularExpression(@"^(ToAssign|ToAccept|Accepted|Located|InProgress|Completed|Cancelled|Paid)$", ErrorMessage = "Status is not valid.")]
        string Status,

        [Required][StringLength(512, MinimumLength = 8)]
        string IssueLocation,

        [Required][StringLength(512, MinimumLength = 4)]
        string Destination,
        
        [Required][RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string Issuer,

        [Required][StringLength(512, MinimumLength = 4)]
        string Details,

        [Required][StringLength(64, MinimumLength = 8)]
        string Name,

        [Required][StringLength(2048, MinimumLength = 8)]
        string Image,

        [Required][RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string Policy,

        [Required][RegularExpression(@"^(0?4(14|24|16|26)\d{7})$", ErrorMessage = "Invalid phone number format.")]
        string PhoneNumber,

        [Required][Range(999999, 100000000)]
        int IdentificationNumber
    );
}