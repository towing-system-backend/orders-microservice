using Application.Core;
using MassTransit.Transports;
using MassTransit;
using orders_microservice.Application.Commands.UpdateOrder.types;
using orders_microservice.Application.Errors;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;
using orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.Events;

namespace orders_microservice.Application.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler(
    IEventStore eventStore,
    IOrderRepository orderRepository,
    IMessageBrokerService messageBrokerService,
    IPublishEndpoint publishEndpoint
)
    : IService<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
{
    private readonly IEventStore _eventStore = eventStore;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task<Result<UpdateOrderStatusResponse>> Execute(UpdateOrderStatusCommand command)
    {
        var orderRegistered = await _orderRepository.FindById(command.Id);
        if (!orderRegistered.HasValue()) return Result<UpdateOrderStatusResponse>.MakeError(new OrderNotFoundError());
        var order = orderRegistered.Unwrap();

        if (command.Status != "Cancelled")
        {
            order.UpdateOrderStatus(new OrderStatus(command.Status));
            await _publishEndpoint.Publish(new UpdateOrderStatusEvent(Guid.Parse(order.GetOrderId.GetValue())));
        }

        if (command.Status == "Cancelled")
        {
            order.UpdateOrderStatus(new OrderStatus(command.Status));
            await _publishEndpoint.Publish(new OrderCancelledEvent(Guid.Parse(order.GetOrderId.GetValue())));
        }

        var events = order.PullEvents();
        await _orderRepository.Save(order);
        await _eventStore.AppendEvents(events);
        await _messageBrokerService.Publish(events);
        return Result<UpdateOrderStatusResponse>.MakeSuccess(new UpdateOrderStatusResponse(command.Id));
    }

}