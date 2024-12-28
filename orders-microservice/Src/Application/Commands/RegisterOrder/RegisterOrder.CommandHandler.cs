using Application.Core;
using MassTransit;
using orders_microservice.Application.Commands.RegisterOrder.types;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;
using orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.Events;


namespace orders_microservice.Application.Commands.RegisterOrder;

public class RegisterOrderCommandHandler(
    IdService<string> idService,
    IMessageBrokerService messageBrokerService,
    IEventStore eventStore,
    IOrderRepository orderRepository,
    IPublishEndpoint publishEndpoint
    ) :
    IService<RegisterOrderCommand, RegisterOrderResponse>
{
    private readonly IdService<string> _idService = idService;
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
    private readonly IEventStore _eventStore = eventStore;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task<Result<RegisterOrderResponse>> Execute(RegisterOrderCommand command)
    {
        var id = _idService.GenerateId();
        var order = Order.Create(
            new OrderId(id),
            new OrderStatus(command.Status),
            new OrderIssueLocation(command.IssueLocation),
            new OrderDestinationLocation(command.Destination),
            new OrderTowDriverAssigned("Not assigned"),
            new OrderDetails(command.Details),
            new OrderClientInformation(command.Name, command.Image, command.Policy, command.PhoneNumber)
        );

        var events = order.PullEvents();
        await _publishEndpoint.Publish(new OrderCreatedEvent(Guid.Parse(id)));
        await _orderRepository.Save(order);
        await _eventStore.AppendEvents(events);
        await _messageBrokerService.Publish(events);

        return Result<RegisterOrderResponse>.MakeSuccess(new RegisterOrderResponse(id));
    }
}