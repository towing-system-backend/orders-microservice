using Application.Core;

namespace Order.Domain
{
    public class CreateAdditionalCostEvent(string publisherId, string type, AdditionalCostCreated context) : DomainEvent(publisherId, type, context) { }
    public class AdditionalCostCreated
    (
        string id,
        decimal amount,
        string category,
        string name
    )
    {
        public readonly string Id = id;
        public readonly decimal Amount = amount;
        public readonly string Category = category;
        public readonly string Name = name;

        public static CreateAdditionalCostEvent CreateEvent(
            OrderId publisherId, 
            AdditionalCostId id,
            AdditionalCostAmount amount,
            AdditionalCostCategory category,
            AdditionalCostName name
        )
        {
            return new CreateAdditionalCostEvent(
                publisherId.GetValue(),
                typeof(AdditionalCostCreated).Name,
                new AdditionalCostCreated(
                    id.GetValue(),
                    amount.GetValue(),
                    category.GetValue(),
                    name.GetValue()
                )
            );
        }
    }
}