using System.ComponentModel.DataAnnotations;

namespace orders_microservice.Infrastructure.Controllers.Dtos;

public record UpdateOrderStatusDto
{
    
    [Required] [StringLength(128, MinimumLength = 4)]
    public string Id { get; init; }

    [Required] [StringLength(64, MinimumLength = 4)]
    public string Status { get; init; }
    
}