﻿using Application.Core;
using MassTransit;
using orders_microservice.Application.Commands.UpdateOrder.types;
using orders_microservice.Application.Errors;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;
using orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects;
using orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.Events;

namespace orders_microservice.Application.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(
    IdService<string> idService,
    IEventStore eventStore,
    IOrderRepository orderRepository,
    IMessageBrokerService messageBrokerService,
    IPublishEndpoint publishEndpoint
)
    : IService<UpdateOrderCommand, UpdateOrderResponse>
{
    private readonly IdService<string> _idService = idService;
    private readonly IEventStore _eventStore = eventStore;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task<Result<UpdateOrderResponse>> Execute(UpdateOrderCommand command)
    {
        var orderRegistered = await _orderRepository.FindById(command.Id);
        if (!orderRegistered.HasValue()) return Result<UpdateOrderResponse>.MakeError(new OrderNotFoundError());
        var order = orderRegistered.Unwrap();

        if(command.Status != null && command.Status != "Cancelled") 
        {
            order.UpdateOrderStatus(new OrderStatus(command.Status));
            await _publishEndpoint.Publish(new UpdateOrderStatusEvent(Guid.Parse(order.GetOrderId.GetValue())));
        }

        if(command.Status != null && command.Status == "Cancelled") 
        {
            order.UpdateOrderStatus(new OrderStatus(command.Status));
            await _publishEndpoint.Publish(new OrderCancelledEvent(Guid.Parse(order.GetOrderId.GetValue())));
        }

        if (command.TowDriverAssigned != null) order.UpdateOrderTowDriverAssigned(new OrderTowDriverAssigned(command.TowDriverAssigned));
        if (command.Destination != null) order.UpdateOrderDestinationLocation(new OrderDestinationLocation(command.Destination));
        if (command.AdditionalCosts != null)
            foreach (var cost in command.AdditionalCosts)
            {
                order.CreateAdditionalCost(
                    new AdditionalCostId(_idService.GenerateId()),
                    new AdditionalCostName(cost.Name),
                    new AdditionalCostCategory(cost.Category),
                    new AdditionalCostAmount(cost.Amount)
                );
            }

        var events = order.PullEvents();
        await _orderRepository.Save(order);
        await _eventStore.AppendEvents(events);
        await _messageBrokerService.Publish(events);
        return Result<UpdateOrderResponse>.MakeSuccess(new UpdateOrderResponse(command.Id));
    }
}