namespace orders_microservice.Src.Application.Commands.RemoveAdditionalCost.Types
{
    public record RemoveAdditionalCostCommand
    (
        string OrderId,
        string AdditionalCostId
    );
}
