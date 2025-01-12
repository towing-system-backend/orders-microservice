using Application.Core;
using MassTransit;
using Order.Domain;

namespace Order.Application
{
    public class UpdateOrderCommandHandler(
        IdService<string> idService,
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IMessageBrokerService messageBrokerService,
        IPublishEndpoint publishEndpoint
    ) : IService<UpdateOrderCommand, UpdateOrderResponse>
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

            if (command.Status != null && command.Status != "Cancelled")
            {
                order.UpdateOrderStatus(new OrderStatus(command.Status));
                await _publishEndpoint.Publish(new EventUpdateOrderStatus(Guid.Parse(order.GetOrderId().GetValue())));
            }

            if (command.Status != null && command.Status == "Cancelled")
            {
                order.UpdateOrderStatus(new OrderStatus(command.Status));
                await _publishEndpoint.Publish(new EventOrderCancelled(Guid.Parse(order.GetOrderId().GetValue())));
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
            await Task.WhenAll
            (
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );

            return Result<UpdateOrderResponse>.MakeSuccess(new UpdateOrderResponse(command.Id));
        }
    }
}