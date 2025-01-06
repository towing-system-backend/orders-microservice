using System.ComponentModel.DataAnnotations;

namespace Order.Infrastructure
{
    public record CreateOrderDto
    (
        [Required][StringLength(64, MinimumLength = 4)]
        string Status,

        [Required][StringLength(512, MinimumLength = 8)]
        string IssueLocation,

        [Required][StringLength(512, MinimumLength = 4)]
        string Destination,

        [Required][StringLength(512, MinimumLength = 4)]
        string Details,

        [Required][StringLength(64, MinimumLength = 8)]
        string Name,

        [Required][StringLength(2048, MinimumLength = 8)]
        string Image,

        [Required][StringLength(128, MinimumLength = 32)]
        string Policy,

        [Required][StringLength(64, MinimumLength = 8)]
        string PhoneNumber
    );
}