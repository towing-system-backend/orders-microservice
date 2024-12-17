using Application.Core;
using MongoDB.Driver;
using orders_microservice.Application.Commands.RegisterOrder.types;
using orders_microservice.Domain.Repositories;
using orders_microservice.Domain.ValueObjects;


namespace orders_microservice.Application.Commands.RegisterOrder;

public class RegisterOrderCommandHandler(
    IdService<string> idService,
    IMessageBrokerService messageBrokerService,
    IEventStore eventStore,
    IOrderRepository orderRepository) :
    IService<RegisterOrderCommand, RegisterOrderResponse>
{
    private readonly IdService<string> IdService = idService;
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
    private readonly IEventStore EventStore = eventStore;
    private readonly IOrderRepository OrderRepository = orderRepository;
    public async Task<Result<RegisterOrderResponse>> Execute(RegisterOrderCommand command)
    {
        var id = IdService.GenerateId();
        var order = Order.Create(
            new OrderId(id),
            new OrderStatus(command.status),
            new OrderIssueLocation(command.issueLocation),
            new OrderDestinationLocation(command.destination),
            new OrderTowDriverAssigned("Not assigned"),
            new OrderDetails(command.details),
            new OrderClientInformation(command.name, command.image, command.policy, command.phoneNumber)
        );

        var events = order.PullEvents();
        await OrderRepository.Save(order);
        await EventStore.AppendEvents(events);
        await _messageBrokerService.Publish(events);

        return Result<RegisterOrderResponse>.MakeSuccess(new RegisterOrderResponse(id));
    }
}