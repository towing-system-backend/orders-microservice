using Application.Core;
using orders_microservice.Application.Errors;
using orders_microservice.Domain.Repositories;
using orders_microservice.Src.Application.Commands.RemoveAdditionalCost.Types;
using orders_microservice.Src.Application.Errors;
using orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects;

namespace orders_microservice.Src.Application.Commands.RemoveAdditionalCost
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

            if (!order.GetAdditionalCosts.Any(x => x.GetAdditionalCostId.GetValue() == command.AdditionalCostId))
                return Result<RemoveAdditionalCostResponse>.MakeError(new AdditionalCostNotFoundError());
            
            order.RemoveAdditionalCost(new AdditionalCostId(command.AdditionalCostId));
            var events = order.PullEvents();
            await _orderRepository.Save(order);
            await _eventStore.AppendEvents(events);
            await _messageBrokerService.Publish(events);
            return Result<RemoveAdditionalCostResponse>.MakeSuccess(new RemoveAdditionalCostResponse(command.OrderId));
        }
    }
}
