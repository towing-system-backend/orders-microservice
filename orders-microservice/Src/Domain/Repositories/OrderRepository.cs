using Application.Core;

namespace orders_microservice.Domain.Repositories;

public interface IOrderRepository
{
    Task Save(Order order);
    Task<Optional<Order>> FindById(string userId);
}