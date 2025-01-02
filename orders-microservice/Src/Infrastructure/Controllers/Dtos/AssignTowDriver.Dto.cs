using System.ComponentModel.DataAnnotations;

namespace orders_microservice.Infrastructure.Controllers.Dtos;

public record AssignTowDriverDto
{
    [Required][StringLength(128, MinimumLength = 8)]
    public string OrderId { get; init; }

    [Required]
    public Dictionary<string,string> TowsLocation { get; init; }
};