using System.ComponentModel.DataAnnotations;

namespace Order.Infrastructure
{
    public record FindOrderByStatusDto
    (
        [Required][RegularExpression(@"^(ToaAssign|ToAccept|Accepted|Located|InProgress|Completed|Cancelled|Paid)$", ErrorMessage = "Status is not valid.")]
        string Status
    );
}