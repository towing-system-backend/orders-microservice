using System.ComponentModel.DataAnnotations;

namespace orders_microservice.Infrastructure.Controllers.Dtos;

public record FindOrderByStatusDto
{
    [Required]
    [StringLength(64, MinimumLength = 4)]
    public string Status { get; init; }
}