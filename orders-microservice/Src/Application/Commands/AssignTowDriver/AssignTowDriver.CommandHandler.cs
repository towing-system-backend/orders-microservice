using Application.Core;
using System.Text.Json.Nodes;
using Order.Domain;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Contracts;

namespace Order.Application
{
    public class AssignTowDriverCommandHandler
    (
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IPublishEndPointService publishEndpoint,
        IMessageBrokerService messageBrokerService,
        ILocationService<JsonNode> locationService,
        ISagaStateMachineService<string> sagaRepository
    ) : IService<AssignTowDriverCommand, AssignTowDriverResponse>
    {
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IPublishEndPointService _publishEndpoint = publishEndpoint;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly ILocationService<JsonNode> _locationService = locationService;
        private readonly ISagaStateMachineService<string> _sagaRepository = sagaRepository;
        public async Task<Result<AssignTowDriverResponse>> Execute(AssignTowDriverCommand command)
        {
            var orderRegistered = await _orderRepository.FindById(command.OrderId);
            if (!orderRegistered.HasValue())
                return Result<AssignTowDriverResponse>.MakeError(new OrderNotFoundError());

            var order = orderRegistered.Unwrap();

            var orderLocation = order.GetOrderIssueLocation().GetValue();
            var drivers = (await _locationService.FindNearestTow(command.TowsLocation, orderLocation))["TowLocations"]
                ?.AsArray()
                .Select(l => new AssignTowDriverResponse(
                    l?["TowDriverId"]?.ToString() ?? string.Empty,
                    l["Latitude"].GetValue<double>(),
                    l["Longitude"].GetValue<double>(),
                    l["Distance"].GetValue<double>(),
                    l["Address"]?.ToString(),
                    l["EstimatedTimeOfArrival"]?.ToString()
                ))
                .ToList();

           
            if (drivers.IsNullOrEmpty()) return Result<AssignTowDriverResponse>.MakeError(new NoAvailableDriversError());
            var rejectedDrivers = await _sagaRepository.FindRejectedDrivers(command.OrderId);
            var availableDrivers = drivers
                .Where(d => !rejectedDrivers.Contains(d.TowDriverId))
                .ToList();
            var driver = availableDrivers.FirstOrDefault();
      
            var token = command.DriversDeviceInfo.GetValueOrDefault(driver.TowDriverId);
            if (string.IsNullOrEmpty(token)) return Result<AssignTowDriverResponse>.MakeError(new MissingDriverTokenError());
            order.UpdateOrderStatus(new OrderStatus("ToAccept"));
            order.UpdateOrderTowDriverAssigned(new OrderTowDriverAssigned(driver.TowDriverId));
            var routeInfo = await _locationService.FindShortestRoute(
                order.GetOrderIssueLocation().GetValue(),
                order.GetOrderDestinationLocation().GetValue()
            );
            var distanceToDestination = routeInfo?["Distance"].GetValue<double>();
            var totalDistance = driver.Distance + distanceToDestination;
            order.UpdateOrderTotalDistance(new OrderTotalDistance(totalDistance ?? 0));

            var events = order.PullEvents();
            await Task.WhenAll(
                _publishEndpoint.Publish(
                    new EventOrderToAccept(
                        Guid.Parse(order.GetOrderId().GetValue()),
                        driver.TowDriverId,
                        token,
                        DateTime.Now
                    )
                ),
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );

            return Result<AssignTowDriverResponse>.MakeSuccess(driver);
        }
    }
}
