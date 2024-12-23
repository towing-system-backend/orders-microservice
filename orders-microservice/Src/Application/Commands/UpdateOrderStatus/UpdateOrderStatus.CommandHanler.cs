using Application.Core;
using orders_microservice.Application.Commands.UpdateOrder.types;
using orders_microservice.Application.Errors;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;

namespace orders_microservice.Application.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler(
    IEventStore eventStore, 
    IOrderRepository orderRepository,
    IMessageBrokerService messageBrokerService
) 
    : IService<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
{ 
    private readonly IEventStore _eventStore = eventStore;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
    public async Task<Result<UpdateOrderStatusResponse>> Execute(UpdateOrderStatusCommand command)
    {
        var orderRegistered = await _orderRepository.FindById(command.Id);
        if (!orderRegistered.HasValue()) return Result<UpdateOrderStatusResponse>.MakeError(new OrderNotFoundError());
        var order = orderRegistered.Unwrap();
        if (string.IsNullOrEmpty(command.Status)) order.UpdateOrderStatus(new OrderStatus(command.Status));
        var events = order.PullEvents();
        await _orderRepository.Save(order);
        await _eventStore.AppendEvents(events);
        await _messageBrokerService.Publish(events);
        return Result<UpdateOrderStatusResponse>.MakeSuccess(new UpdateOrderStatusResponse(command.Id));
    }
    
}