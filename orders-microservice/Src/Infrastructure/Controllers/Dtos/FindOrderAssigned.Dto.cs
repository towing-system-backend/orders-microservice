using System.ComponentModel.DataAnnotations;

namespace orders_microservice.Infrastructure.Controllers.Dtos;

public record FindOrderAssignedDto
{
    [Required] [StringLength(128, MinimumLength = 4)]
    public string Id;
}