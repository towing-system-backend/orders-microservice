using Application.Core;
using Order.Domain;

namespace Order.Application
{
    public class RemoveAdditionalCostCommandHandler
    (
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IMessageBrokerService messageBrokerService
    ) : IService<RemoveAdditionalCostCommand, RemoveAdditionalCostResponse>

    {
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;

        public async Task<Result<RemoveAdditionalCostResponse>> Execute(RemoveAdditionalCostCommand command)
        {
            var orderRegistered = await _orderRepository.FindById(command.OrderId);
            if (!orderRegistered.HasValue()) return Result<RemoveAdditionalCostResponse>.MakeError(new OrderNotFoundError());
            var order = orderRegistered.Unwrap();

            if (order.GetAdditionalCosts()!.All(x => x.GetAdditionalCostId().GetValue() != command.AdditionalCostId))
                return Result<RemoveAdditionalCostResponse>.MakeError(new AdditionalCostNotFoundError());
            
            order.RemoveAdditionalCost(new AdditionalCostId(command.AdditionalCostId));
            var events = order.PullEvents();
            await Task.WhenAll
            (
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );
            return Result<RemoveAdditionalCostResponse>.MakeSuccess(new RemoveAdditionalCostResponse(command.OrderId));
        }
    }
}