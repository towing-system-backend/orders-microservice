using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace order.Infrastructure.Responses;

public record FindOrderAssignedResponse
{
    [Required] [StringLength(64, MinimumLength = 4)]
    public string Status { get; init; }
    
    [Required] [StringLength(512, MinimumLength = 8)]
    public string IssueLocation { get; init; }

    [Required] [StringLength(512, MinimumLength = 4)]
    public string Destination { get; init; }
    
    [Required] [StringLength(512, MinimumLength = 4)]
    public string Details { get; init; }
    
    [Required] [StringLength(64, MinimumLength = 4)]
    public string Name { get; init; }

    [Required] [StringLength(2048, MinimumLength = 8)]
    public string? Image { get; init; }
    
    [Required] [StringLength(64, MinimumLength = 8)]
    public string PhoneNumber { get; init; }
    
    [StringLength(128, MinimumLength = 4)]
    public string? TowDriverAssigned { get; init; }
}