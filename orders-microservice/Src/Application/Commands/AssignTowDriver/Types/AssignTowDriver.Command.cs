namespace orders_microservice.Src.Application.Commands.AssignTowDriver.Types
{
    public record AssignTowDriverCommand(string OrderId, Dictionary<string, string> towsLocation);
}
