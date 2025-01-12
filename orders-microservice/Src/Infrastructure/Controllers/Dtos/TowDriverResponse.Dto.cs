using System.ComponentModel.DataAnnotations;

namespace Order.Infrastructure
{
    public interface IDto { }
    public record TowDriverResponseDto
    (
        [Required][RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string OrderId,

        [RegularExpression(@"^(ToAssign|ToAccept|Accepted|Located|InProgress|Completed|Cancelled|Paid)$", ErrorMessage = "Status is not valid.")]
        string Status,

        [StringLength(16)]
        string? Response
    ): IDto;
   
}
