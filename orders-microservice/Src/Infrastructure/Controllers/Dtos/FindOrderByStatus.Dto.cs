using System.ComponentModel.DataAnnotations;

namespace Order.Infrastructure
{
    public record FindOrderByStatusDto
    (
        [Required][RegularExpression(@"^(Active|Inactive)$", ErrorMessage = "Status must be 'Active', or 'Inactive'.")]
        string Status
    );
}