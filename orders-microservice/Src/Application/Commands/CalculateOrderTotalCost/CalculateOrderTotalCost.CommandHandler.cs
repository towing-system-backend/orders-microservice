using Application.Core;
using Order.Domain;
using Order.Domain.Services;

namespace Order.Application
{
    public class CalculateOrderTotalCostCommandHandler
        (
            IEventStore eventStore,
            IOrderRepository orderRepository,
            IMessageBrokerService messageBrokerService
        ) : IService<CalculateOrderTotalCostCommand, CalculateOrderTotalCostResponse>
    {
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IDomainService<List<AdditionalCost>, OrderTotalCost> _calculateTotalCost = new CalculateTotalCostService();
        public async Task<Result<CalculateOrderTotalCostResponse>> Execute(CalculateOrderTotalCostCommand command)
        {
            decimal totalCost = 0;
            decimal cost = 0;
            var orderRegistred = await _orderRepository.FindById(command.OrderId);
            if (!orderRegistred.HasValue())
                return Result<CalculateOrderTotalCostResponse>.MakeError(new OrderNotFoundError());
            var order = orderRegistred.Unwrap();

            if (order.GetAdditionalCosts()?.Count() > 0)
            {
                cost = _calculateTotalCost.Execute(order.GetAdditionalCosts()!).GetValue();
                totalCost = Convert.ToDecimal(command.CoverageAmount) - cost;
            }

            order.UpdateOrderTotalCost(new OrderTotalCost(totalCost));

            var events = order.PullEvents();
            await Task.WhenAll(
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );

            return Result<CalculateOrderTotalCostResponse>.MakeSuccess(
                new CalculateOrderTotalCostResponse(
                    command.CoverageAmount,
                    cost,
                    (command.CoverageDistance - order.GetOrderTotalDistance().GetValue()) / 1000,
                    totalCost > 0 ? 0 : totalCost
                )
            );
        }
    }
}
