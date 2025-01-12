using Application.Core;
using MassTransit;
using Order.Domain;

namespace Order.Application
{
    public class UpdateOrderStatusCommandHandler(
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IMessageBrokerService messageBrokerService,
        IPublishEndpoint publishEndpoint
    ) : IService<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
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
                await _publishEndpoint.Publish(new EventUpdateOrderStatus(Guid.Parse(order.GetOrderId().GetValue())));
            }

            if (command.Status == "Cancelled")
            {
                order.UpdateOrderStatus(new OrderStatus(command.Status));
                await _publishEndpoint.Publish(new EventOrderCancelled(Guid.Parse(order.GetOrderId().GetValue())));
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