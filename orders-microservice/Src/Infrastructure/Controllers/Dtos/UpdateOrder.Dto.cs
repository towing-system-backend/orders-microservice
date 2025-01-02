using System.ComponentModel.DataAnnotations;

namespace orders_microservice.Infrastructure.Controllers.Dtos;

public record UpdateOrderDto
{
    
    [Required] [StringLength(128, MinimumLength = 4)]
    public string Id;

    [StringLength(64, MinimumLength = 4)]
    public string? Status;
    
    [StringLength(128, MinimumLength = 4)]
    public string? TowDriverAssigned;
    
    [StringLength(512, MinimumLength = 4)]
    public string? Destination;

    public List<AdditionalCostDto> AdditionalCosts = new List<AdditionalCostDto>();

}

public record AdditionalCostDto
{
    [Required][StringLength(64, MinimumLength = 4)]
    public string Name;

    [Required][StringLength(64, MinimumLength = 4)]
    public string Category;

    [Range(0.01, double.MaxValue)]
    public decimal? Amount;
}