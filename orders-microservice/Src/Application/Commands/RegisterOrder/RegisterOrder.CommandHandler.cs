using Application.Core;
using MassTransit;
using Order.Domain;

namespace Order.Application
{
    public class RegisterOrderCommandHandler(
    IdService<string> idService,
    IMessageBrokerService messageBrokerService,
    IEventStore eventStore,
    IOrderRepository orderRepository,
    IPublishEndpoint publishEndpoint
    ) : IService<RegisterOrderCommand, RegisterOrderResponse>
    {
        private readonly IdService<string> _idService = idService;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        public async Task<Result<RegisterOrderResponse>> Execute(RegisterOrderCommand command)
        {
            var id = _idService.GenerateId();
            var order = Domain.Order.Create(
                new OrderId(id),
                new OrderStatus(command.Status),
                new OrderIssueLocation(command.IssueLocation),
                new OrderDestinationLocation(command.Destination),
                new OrderIssuer(command.Issuer),
                new OrderTowDriverAssigned("Not assigned"),
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
                _publishEndpoint.Publish(new EventOrderCreated(Guid.Parse(id))),
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );

            return Result<RegisterOrderResponse>.MakeSuccess(new RegisterOrderResponse(id));
        }
    }
}