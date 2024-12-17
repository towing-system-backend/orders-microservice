using Application.Core;
using orders_microservice.Application.Commands.UpdateOrder.types;
using orders_microservice.Application.Errors;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;

namespace orders_microservice.Application.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(
    IEventStore eventStore,
    IOrderRepository orderRepository,
    IMessageBrokerService messageBrokerService
) 
    : IService<UpdateOrderCommand, UpdateOrderResponse>
{ 
    private readonly IEventStore _eventStore = eventStore;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
    public async Task<Result<UpdateOrderResponse>> Execute(UpdateOrderCommand command)
    {
        var OrderRegistered = await _orderRepository.FindById(command.Id);
        if (!OrderRegistered.HasValue()) return Result<UpdateOrderResponse>.MakeError(new OrderNotFoundError());
        var Order = OrderRegistered.Unwrap();
        if (command.Status != null) Order.UpdateOrderStatus(new OrderStatus(command.Status));
        if (command.TowDriverAssigned != null) Order.UpdateOrderTowDriverAssigned(new OrderTowDriverAssigned(command.TowDriverAssigned));
        if (command.Destination != null) Order.UpdateOrderDestinationLocation(new OrderDestinationLocation(command.Destination));
        var events = Order.PullEvents();
        await _orderRepository.Save(Order);
        await _eventStore.AppendEvents(events);
        await _messageBrokerService.Publish(events);
        return Result<UpdateOrderResponse>.MakeSuccess(new UpdateOrderResponse(command.Id));
    }
}