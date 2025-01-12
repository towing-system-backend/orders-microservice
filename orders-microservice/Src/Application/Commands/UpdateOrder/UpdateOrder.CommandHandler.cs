using Application.Core;
using MassTransit.Transports;
using Newtonsoft.Json.Linq;
using Order.Domain;
using Order.Domain.Services;
using RabbitMQ.Contracts;


namespace Order.Application
{
    public class UpdateOrderCommandHandler(
            IdService<string> idService,
            IEventStore eventStore,
            IOrderRepository orderRepository,
            IMessageBrokerService messageBrokerService,
            IPublishEndPointService publishEndpoint
        ) : IService<UpdateOrderCommand, UpdateOrderResponse>
    {
        private readonly IdService<string> _idService = idService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IPublishEndPointService _publishEndpoint = publishEndpoint;
        private readonly IDomainService<List<string>, bool> _validateNextState = new ValidateNextState();

        public async Task<Result<UpdateOrderResponse>> Execute(UpdateOrderCommand command)
        {
            var orderRegistered = await _orderRepository.FindById(command.Id);
            if (!orderRegistered.HasValue()) return Result<UpdateOrderResponse>.MakeError(new OrderNotFoundError());
            var order = orderRegistered.Unwrap();

            if (command.Status != null)
            {
                if (!_validateNextState.Execute(new List<string>{order.GetOrderStatus().GetValue(), command.Status }))
                    return Result<UpdateOrderResponse>.MakeError(new OrderStatusBadTransitionError(order.GetOrderStatus().GetValue()));
                var eventStatus = new EventSelector(_publishEndpoint);
                await eventStatus.ExecuteEvent(command.Status, order.GetOrderId().GetValue());
                order.UpdateOrderStatus(new OrderStatus(command.Status));
            }

            if (command.TowDriverAssigned != null)
            {
                if (!_validateNextState.Execute(new List<string> { order.GetOrderStatus().GetValue(), "ToAccept"}))
                    return Result<UpdateOrderResponse>.MakeError(new OrderStatusBadTransitionError(order.GetOrderStatus().GetValue()));
                
                await _publishEndpoint.Publish(
                   new EventOrderToAccept(
                       Guid.Parse(order.GetOrderId().GetValue()),
                       command.TowDriverAssigned,
                       "null",
                       DateTime.Now
                   )
                );

                order.UpdateOrderStatus(new OrderStatus("ToAccept"));
                order.UpdateOrderTowDriverAssigned(new OrderTowDriverAssigned(command.TowDriverAssigned));
            }

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