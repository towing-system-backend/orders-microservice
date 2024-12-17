using Application.Core;
using orders_microservice.Application.Commands.UpdateOrder.types;
using orders_microservice.Application.Errors;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;

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
        var OrderRegistered = await _orderRepository.FindById(command.Id);
        if (!OrderRegistered.HasValue()) return Result<UpdateOrderStatusResponse>.MakeError(new OrderNotFoundError());
        var Order = OrderRegistered.Unwrap();
        if (command.status != null) Order.UpdateOrderStatus(new OrderStatus(command.status));
        var events = Order.PullEvents();
        await _orderRepository.Save(Order);
        await _eventStore.AppendEvents(events);
        await _messageBrokerService.Publish(events);
        return Result<UpdateOrderStatusResponse>.MakeSuccess(new UpdateOrderStatusResponse(command.Id));
    }
    
}