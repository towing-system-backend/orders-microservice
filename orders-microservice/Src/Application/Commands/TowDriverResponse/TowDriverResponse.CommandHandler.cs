using Application.Core;
using MassTransit.Saga;
using MassTransit.Transports;
using MassTransit;
using Order.Domain;
using orders_microservice.Utils.Core.Src.Application.NotificationService;
using System.Text.Json.Nodes;

namespace Order.Application
{
    public class TowDriverResponseCommandHandler
    (
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint,
        IMessageBrokerService messageBrokerService,
        INotificationService notificationService
    ) : IService<TowDriverResponseCommand, TowDriverResponseResponse>
    {
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly INotificationService _notificationService = notificationService;
        public async Task<Result<TowDriverResponseResponse>> Execute(TowDriverResponseCommand command)
        {
            var orderRegistered = await _orderRepository.FindById(command.OrderId);
            if (!orderRegistered.HasValue())
                return Result<TowDriverResponseResponse>.MakeError(new OrderNotFoundError());
            var order = orderRegistered.Unwrap();
            if(command.Status == "Rejected" || command.Response == "Rejected")
            {
                await _publishEndpoint.Publish(
                    new EventOrderRejected(
                        Guid.Parse(order.GetOrderId().GetValue()),
                        order.GetOrderTowDriverAssigned()!.GetValue()!
                    )
                );
                order.UpdateOrderStatus(new OrderStatus("ToAssign"));
                order.UpdateOrderTowDriverAssigned(new OrderTowDriverAssigned("Not Assigned"));
                order.UpdateOrderTotalDistance(new OrderTotalDistance(0));
            }

            if(command.Response == "Accepted")
            {
                await _publishEndpoint.Publish(new EventUpdateOrderStatus(Guid.Parse(order.GetOrderId().GetValue())));
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
