using Application.Core;
using Order.Domain;
using Order.Domain.Services;


namespace Order.Application
{
    public class UpdateOrderStatusCommandHandler(
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IMessageBrokerService messageBrokerService,
        IPublishEndPointService publishEndpoint
    ) : IService<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
    {
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IPublishEndPointService _publishEndpoint = publishEndpoint;
        private readonly IDomainService<List<string>, bool> _validateNextState = new ValidateNextState();
        public async Task<Result<UpdateOrderStatusResponse>> Execute(UpdateOrderStatusCommand command)
        {
            var orderRegistered = await _orderRepository.FindById(command.Id);
            if (!orderRegistered.HasValue()) return Result<UpdateOrderStatusResponse>.MakeError(new OrderNotFoundError());
            var order = orderRegistered.Unwrap();

            if (command.Status != null)
            {
                if (!_validateNextState.Execute(new List<string> { order.GetOrderStatus().GetValue(), command.Status }))
                    return Result<UpdateOrderStatusResponse>.MakeError(new OrderStatusBadTransitionError(order.GetOrderStatus().GetValue()));
                var eventStatus = new EventSelector(_publishEndpoint);
                await eventStatus.ExecuteEvent(command.Status, order.GetOrderId().GetValue());
                order.UpdateOrderStatus(new OrderStatus(command.Status));
            }

            var events = order.PullEvents();
            await Task.WhenAll
            (
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );
            return Result<UpdateOrderStatusResponse>.MakeSuccess(new UpdateOrderStatusResponse(command.Id));
        }
    }
}