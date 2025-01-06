using System.ComponentModel.DataAnnotations;

namespace Order.Infrastructure
{
    public record AdditionalCostDto
    (
        [Required][StringLength(64, MinimumLength = 4)]
        string Name,

        [Required][StringLength(64, MinimumLength = 4)]
        string Category,

        [Range(0.01, double.MaxValue)]
        decimal? Amount
    );

    public record UpdateOrderDto
    (

        [Required][RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string Id,

        [RegularExpression(@"^(Active|Inactive)$", ErrorMessage = "Status must be 'Active', or 'Inactive'.")]
        string? Status,

        [RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string? TowDriverAssigned,

        [StringLength(512, MinimumLength = 4)]
        string? Destination,

        List<AdditionalCostDto> AdditionalCosts
    );
}
