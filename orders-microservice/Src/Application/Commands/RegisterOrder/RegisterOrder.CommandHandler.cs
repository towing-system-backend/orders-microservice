using Application.Core;
using Order.Domain;
using RabbitMQ.Contracts;

namespace Order.Application
{
    public class RegisterOrderCommandHandler(
    IdService<string> idService,
    IMessageBrokerService messageBrokerService,
    IEventStore eventStore,
    IOrderRepository orderRepository,
    IPublishEndPointService publishEndpoint
    ) : IService<RegisterOrderCommand, RegisterOrderResponse>
    {
        private readonly IdService<string> _idService = idService;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IPublishEndPointService _publishEndpoint = publishEndpoint;
        public async Task<Result<RegisterOrderResponse>> Execute(RegisterOrderCommand command)
        {
            var id = _idService.GenerateId();
            var order = Domain.Order.Create(
                new OrderId(id),
                new OrderStatus(command.Status),
                new OrderIssueLocation(command.IssueLocation),
                new OrderDestinationLocation(command.Destination),
                new OrderTowDriverAssigned("Not Assigned."),
                new OrderIssuer(command.Issuer),
                new OrderDetails(command.Details),
                new OrderClientInformation
                    (command.Name, command.Image, command.Policy, command.PhoneNumber, command.IdentificationNumber),
                new OrderTotalCost(0),
                new OrderTotalDistance(0),
                null
            );
            var events = order.PullEvents();
            await Task.WhenAll
            (
                _publishEndpoint.Publish(new EventOrderToAssign(Guid.Parse(id), DateTime.Now)),
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );

            return Result<RegisterOrderResponse>.MakeSuccess(new RegisterOrderResponse(id));
        }
    }
}