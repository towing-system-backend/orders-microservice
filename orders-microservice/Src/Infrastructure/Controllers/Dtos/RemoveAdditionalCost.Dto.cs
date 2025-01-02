using System.ComponentModel.DataAnnotations;

namespace orders_microservice.Src.Infrastructure.Controllers.Dtos
{
    public record RemoveAdditionalCostDto
    {
        [Required][StringLength(128, MinimumLength = 4)]
        public string OrderId;

        [Required][StringLength(128, MinimumLength = 4)]
        public string AdditionalCostId;
    }
}
