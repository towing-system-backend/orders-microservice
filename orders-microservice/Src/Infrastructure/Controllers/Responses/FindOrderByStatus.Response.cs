using System.ComponentModel.DataAnnotations;

namespace order.Infrastructure.Responses;

public record FindOrderByStatusResponse
{
    [Required] [StringLength(64, MinimumLength = 4)]
    public string Status { get; init; }

    [Required] [StringLength(512, MinimumLength = 8)]
    public string IssueLocation { get; init; }

    [Required] [StringLength(512, MinimumLength = 4)]
    public string Destination { get; init; }

    [StringLength(128, MinimumLength = 4)]
    public string? TowDriverAssigned { get; init; }

    [Required] [StringLength(512, MinimumLength = 4)]
    public string Details { get; init; }

    [Required] [StringLength(64, MinimumLength = 4)]
    public string Name { get; init; }

    [Required] [StringLength(2048, MinimumLength = 8)]
    public string? Image { get; init; }

    [Required] [StringLength(128, MinimumLength = 32)]
    public string? Policy { get; init; }

    [Required] [StringLength(64, MinimumLength = 8)]
    public string PhoneNumber { get; init; }
}
