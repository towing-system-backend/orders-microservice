using Application.Core;
using System.Text.Json.Nodes;
using Microsoft.IdentityModel.Tokens;

using RabbitMQ.Contracts;

namespace Order.Application
{
    using Order.Domain;
    public class AssignTowDriverCommandHandler : IService<AssignTowDriverCommand, AssignTowDriverResponse>
    {
        private readonly IEventStore _eventStore;
        private readonly IOrderRepository _orderRepository;
        private readonly IPublishEndPointService _publishEndpoint;
        private readonly IMessageBrokerService _messageBrokerService;
        private readonly ILocationService<JsonNode> _locationService;
        private readonly ISagaStateMachineService<string> _sagaRepository;

        public AssignTowDriverCommandHandler(
            IEventStore eventStore,
            IOrderRepository orderRepository,
            IPublishEndPointService publishEndpoint,
            IMessageBrokerService messageBrokerService,
            ILocationService<JsonNode> locationService,
            ISagaStateMachineService<string> sagaRepository
        )
        {
            _eventStore = eventStore;
            _orderRepository = orderRepository;
            _publishEndpoint = publishEndpoint;
            _messageBrokerService = messageBrokerService;
            _locationService = locationService;
            _sagaRepository = sagaRepository;
        }

        public async Task<Result<AssignTowDriverResponse>> Execute(AssignTowDriverCommand command)
        {
            var order = await GetOrderById(command.OrderId);
            if (order == null) return Result<AssignTowDriverResponse>.MakeError(new OrderNotFoundError());

            if (!CanAssignDriver(order))
                return Result<AssignTowDriverResponse>.MakeError(new OrderNotAssignableError());

            var rejectedDrivers = await GetRejectedDrivers(command.OrderId);
            var availableDrivers = await GetAvailableDrivers(command, order, rejectedDrivers);
            if (!availableDrivers.Any())
                return Result<AssignTowDriverResponse>.MakeError(new NoAvailableDriversError());

            var driver = availableDrivers.First();
            var token = command.DriversDeviceInfo.GetValueOrDefault(driver.TowDriverId);
<<<<<<< HEAD
            if (string.IsNullOrEmpty(token))
                return Result<AssignTowDriverResponse>.MakeError(new MissingDriverTokenError());
=======
            if (string.IsNullOrEmpty(token)) 
            {
                await PublishOrderEvents(order, driver, null);
                return Result<AssignTowDriverResponse>.MakeError(new MissingDriverTokenError());
            }
                
>>>>>>> f77c59b17e7f1954a8fcbdd17500f634594610b9

            await UpdateOrderWithDriverInfo(order, driver, token);
            await PublishOrderEvents(order, driver, token);

            return Result<AssignTowDriverResponse>.MakeSuccess(driver);
        }

        private async Task<Order> GetOrderById(string orderId)
        {
            var order = await _orderRepository.FindById(orderId);
            return order.HasValue() ? order.Unwrap() : null;
        }

        private bool CanAssignDriver(Order order)
        {
            return order.GetOrderStatus().GetValue() == "ToAssign";
        }

        private async Task<List<string>> GetRejectedDrivers(string orderId)
        {
            var rejectedDrivers = await _sagaRepository.FindRejectedDrivers(orderId);
            return rejectedDrivers.IsNullOrEmpty() ? new List<string>() : rejectedDrivers;
        }

        private async Task<List<AssignTowDriverResponse>> GetAvailableDrivers(AssignTowDriverCommand command,
            Order order, List<string> rejectedDrivers)
        {
            var orderLocation = order.GetOrderIssueLocation().GetValue();
            var nearestTows = await _locationService.FindNearestTow(command.TowsLocation, orderLocation);
            var drivers = nearestTows["TowLocations"]?.AsArray()
                .Select(l => new AssignTowDriverResponse(
                    l?["TowDriverId"]?.ToString() ?? string.Empty,
                    l["Latitude"].GetValue<double>(),
                    l["Longitude"].GetValue<double>(),
                    l["Distance"].GetValue<double>(),
                    l["Address"]?.ToString(),
                    l["EstimatedTimeOfArrival"]?.ToString()
                ))
                .ToList();

            if (drivers == null) return new List<AssignTowDriverResponse>();

            return drivers.Where(d => !rejectedDrivers.Contains(d.TowDriverId)).ToList();
        }

        private async Task UpdateOrderWithDriverInfo(Order order, AssignTowDriverResponse driver, string token)
        {
            order.UpdateOrderStatus(new OrderStatus("ToAccept"));
            order.UpdateOrderTowDriverAssigned(new OrderTowDriverAssigned(driver.TowDriverId));

            var routeInfo = await _locationService.FindShortestRoute(
                order.GetOrderIssueLocation().GetValue(),
                order.GetOrderDestinationLocation().GetValue()
            );

            var distanceToDestination = routeInfo?["Distance"].GetValue<double>();
            var totalDistance = driver.Distance + distanceToDestination;
            order.UpdateOrderTotalDistance(new OrderTotalDistance(totalDistance ?? 0));
        }

        private async Task PublishOrderEvents(Order order, AssignTowDriverResponse driver, string token)
        {
            var events = order.PullEvents();
            await Task.WhenAll(
                _publishEndpoint.Publish(new EventOrderToAccept(
                    Guid.Parse(order.GetOrderId().GetValue()),
                    driver.TowDriverId,
                    token,
                    DateTime.Now
                )),
                _orderRepository.Save(order),
                _eventStore.AppendEvents(events),
                _messageBrokerService.Publish(events)
            );
        }
    }
}
