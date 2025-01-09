using Application.Core;

namespace Order.Domain
{
    public interface IOrderRepository
    {
        Task Save(Order order);
        Task<Optional<Order>> FindById(string orderId);
    }
}