using System.ComponentModel.DataAnnotations;

namespace orders_microservice.Infrastructure.Controllers.Dtos;

public record UpdateOrderDto
{
    
    [Required] [StringLength(128, MinimumLength = 4)]
    public string Id { get; init; }

    [StringLength(64, MinimumLength = 4)]
    public string? Status { get; init; }
    
    [StringLength(128, MinimumLength = 4)]
    public string? TowDriverAssigned { get; init; }
    
    [StringLength(512, MinimumLength = 4)]
    public string? Destination { get; init; }
    
}