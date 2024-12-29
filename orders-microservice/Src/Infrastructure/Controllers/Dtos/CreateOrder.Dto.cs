using System.ComponentModel.DataAnnotations;

namespace orders_microservice.Infrastructure.Controllers.Dtos;

public record CreateOrderDto
{
    [Required] [StringLength(64, MinimumLength = 4)]
    public string Status;

    [Required] [StringLength(512, MinimumLength = 8)]
    public string IssueLocation;

    [Required] [StringLength(512, MinimumLength = 4)]
    public string Destination;

    [Required] [StringLength(512, MinimumLength = 4)]
    public string Details;

    [Required] [StringLength(64, MinimumLength = 8)]
    public string Name;

    [Required] [StringLength(2048, MinimumLength = 8)]
    public string Image;

    [Required] [StringLength(128, MinimumLength = 32)]
    public string Policy;

    [Required] [StringLength(64, MinimumLength = 8)]
    public string PhoneNumber;
}