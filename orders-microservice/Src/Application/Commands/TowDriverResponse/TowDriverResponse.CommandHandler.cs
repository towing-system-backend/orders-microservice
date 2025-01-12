using Application.Core;
using Order.Domain;
using RabbitMQ.Contracts;

namespace Order.Application
{
    public class TowDriverResponseCommandHandler
    (
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IPublishEndPointService publishEndpoint,
        IMessageBrokerService messageBrokerService
    ) : IService<TowDriverResponseCommand, TowDriverResponseResponse>
    {
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IPublishEndPointService _publishEndpoint = publishEndpoint;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        public async Task<Result<TowDriverResponseResponse>> Execute(TowDriverResponseCommand command)
        {
            var orderRegistered = await _orderRepository.FindById(command.OrderId);
            if (!orderRegistered.HasValue())
                return Result<TowDriverResponseResponse>.MakeError(new OrderNotFoundError());
            var order = orderRegistered.Unwrap();
            if(command.Status == "Rejected" || command.Response == "Rejected")
            {
                await _publishEndpoint.Publish(
                    new EventOrderToAccept(
                        Guid.Parse(order.GetOrderId().GetValue()),
                        order.GetOrderTowDriverAssigned()!.GetValue()!,
                        string.Empty,
                        DateTime.Now
                    )
                );
                order.UpdateOrderStatus(new OrderStatus("ToAssign"));
                order.UpdateOrderTowDriverAssigned(new OrderTowDriverAssigned("Not Assigned"));
                order.UpdateOrderTotalDistance(new OrderTotalDistance(0));
            }

            if(command.Response == "Accepted")
            {
                await _publishEndpoint.Publish(new EventOrderAccepted(Guid.Parse(order.GetOrderId().GetValue()), DateTime.Now));
                order.UpdateOrderStatus(new OrderStatus("Accepted"));
            }
 
            var events = order.PullEvents();
            await Task.WhenAll(
                
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );

            return Result<TowDriverResponseResponse>
                .MakeSuccess(
                    new TowDriverResponseResponse(order.GetOrderId().GetValue(), order.GetOrderStatus().GetValue())
            );
        }
    }
}
