namespace Order.Application
{
    public record AssignTowDriverCommand(string OrderId, Dictionary<string, string> TowsLocation);
}
