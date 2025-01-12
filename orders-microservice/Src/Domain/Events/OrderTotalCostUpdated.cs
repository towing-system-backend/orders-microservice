using Application.Core;


namespace Order.Domain
{
    public class OrderTotalCostUpdatedEvent(string publisherId, string type, OrderTotalCostUpdated context) 
        : DomainEvent(publisherId, type, context){ }
    public class OrderTotalCostUpdated(decimal totalCost)
    {
        public readonly decimal TotalCost = totalCost;
        public static OrderTotalCostUpdatedEvent CreateEvent(OrderId publisherId, OrderTotalCost totalCost) 
        {
            return new OrderTotalCostUpdatedEvent(
                publisherId.GetValue(),
                typeof(OrderTotalCostUpdated).Name,
                new OrderTotalCostUpdated(totalCost.GetValue())         
            );
        }
    }
}
