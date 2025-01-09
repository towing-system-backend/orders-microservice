using Application.Core;
using MassTransit;
using System.Text.Json.Nodes;
using Order.Domain;
using MassTransit.Testing;

namespace Order.Application
{
    public class AssignTowDriverCommandHandler
    (
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint,
        IMessageBrokerService messageBrokerService,
        ILocationService<JsonNode> locationService,
        ISagaStateMachineService<string> sagaRepository
    ) : IService<AssignTowDriverCommand, AssignTowDriverResponse>
    {
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly ILocationService<JsonNode> _locationService = locationService;
        private readonly ISagaStateMachineService<string> _sagaRepository = sagaRepository;
        public async Task<Result<AssignTowDriverResponse>> Execute(AssignTowDriverCommand command)
        {
            var orderRegistered = await _orderRepository.FindById(command.OrderId);
            if (!orderRegistered.HasValue()) return Result<AssignTowDriverResponse>.MakeError(new OrderNotFoundError());
            var order = orderRegistered.Unwrap();
            var res = await _locationService.FindNearestTow(command.TowsLocation, order.GetOrderDestinationLocation().GetValue());

            var drivers = res["TowLocations"]?.AsArray()
                .Select(l => new AssignTowDriverResponse(
                    l?["TowDriverId"]?.ToString() ?? string.Empty,
                    l["Latitude"].GetValue<double>(),
                    l["Longitude"].GetValue<double>(),
                    l["Distance"].GetValue<double>(),
                    l["Address"]?.ToString(),
                    l["EstimatedTimeOfArrival"]?.ToString()
                )
            ).ToList();

            var driverThatRejected = await _sagaRepository.FindRejectedDrivers(command.OrderId);
            drivers = drivers?.Where(d => !driverThatRejected.Contains(d.TowDriverId)).ToList();
            var driver = drivers?.First();

            // Lógica para enviar una notificación al gruero más cercano
            // por hacer: Agregar lógica de notificación

            if (true)
            {
                order.UpdateOrderTowDriverAssigned(new OrderTowDriverAssigned(driver.TowDriverId));
                await _publishEndpoint.Publish(new UpdateOrderStatusEvent(Guid.Parse(order.GetOrderId().GetValue())));
                var events = order.PullEvents();
                await _orderRepository.Save(order);
                await _eventStore.AppendEvents(events);
                await _messageBrokerService.Publish(events);
                return Result<AssignTowDriverResponse>.MakeSuccess(driver);
            }

            // Si el gruero rechaza la orden
            await _publishEndpoint.Publish(
                new OrderRejectedEvent(
                    Guid.Parse(order.GetOrderId().GetValue()),
                    driver.TowDriverId
                )
            );
            return Result<AssignTowDriverResponse>.MakeError(new OrderRejectedByDriverError());
        }
    }
}
