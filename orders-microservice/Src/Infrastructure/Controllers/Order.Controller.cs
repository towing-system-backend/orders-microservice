using Application.Core;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Order.Domain;
using Order.Application; 
using orders_microservice.Utils.Core.Src.Application.NotificationService;




namespace Order.Infrastructure
{
    [ApiController]
    [Route("api/order")]
    public class OrderController(
        IdService<string> idService,
        Logger logger,
        IMessageBrokerService messageBrokerService,
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint,
        INotificationService notificationService,
        ILocationService<JsonNode> locationService,
        ISagaStateMachineService<string> sagaStateMachineService,
        IPerformanceLogsRepository performanceLogsRepository
    ) : ControllerBase
    {
        private readonly IdService<string> _idService = idService;
        private readonly Logger _logger = logger;
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly ILocationService<JsonNode> _locationService = locationService;
        private readonly ISagaStateMachineService<string> _sagaStateMachineService = sagaStateMachineService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IPerformanceLogsRepository _performanceLogsRepository = performanceLogsRepository;

        [HttpPost("create")]
        public async Task<ObjectResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var command = new RegisterOrderCommand(
                createOrderDto.Status,
                createOrderDto.IssueLocation,
                createOrderDto.Destination,
                createOrderDto.Issuer,
                createOrderDto.Details,
                createOrderDto.Name,
                createOrderDto.Image,
                createOrderDto.Policy,
                createOrderDto.PhoneNumber,
                createOrderDto.IdentificationNumber
            );

            var handler =
                new ExceptionCatcher<RegisterOrderCommand, RegisterOrderResponse>(
                    new PerfomanceMonitor<RegisterOrderCommand, RegisterOrderResponse>(
                        new LoggingAspect<RegisterOrderCommand, RegisterOrderResponse>(
                            new RegisterOrderCommandHandler(
                                _idService,
                                _messageBrokerService,
                                _eventStore,
                                _orderRepository,
                                _publishEndpoint
                            ), _logger
                        ), _logger, _performanceLogsRepository, nameof(RegisterOrderCommandHandler), "Write"
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);
            
            return Ok(res.Unwrap());
        }

        [HttpPatch("update")]
        public async Task<ObjectResult> UpdateOrder([FromBody] UpdateOrderDto updateOrderDto)
        {
            var command = new UpdateOrderCommand(
                updateOrderDto.Id,
                updateOrderDto.Status,
                updateOrderDto.TowDriverAssigned,
                updateOrderDto.Destination,
                updateOrderDto.AdditionalCosts.Select(
                    x => new AdditionalCostCommand(
                        x.Name,
                        x.Category,
                        x.Amount!.Value 
                    )
                ).ToList()
            );

            var handler =
                new ExceptionCatcher<UpdateOrderCommand, UpdateOrderResponse>(
                    new PerfomanceMonitor<UpdateOrderCommand, UpdateOrderResponse>(
                        new LoggingAspect<UpdateOrderCommand, UpdateOrderResponse>(
                            new UpdateOrderCommandHandler(
                                _idService,
                                _eventStore,
                                _orderRepository,
                                _messageBrokerService,
                                _publishEndpoint
                            ), _logger
                        ), _logger, _performanceLogsRepository, nameof(UpdateOrderCommandHandler), "Write"

                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);
            
            return Ok(res.Unwrap());
        }

        [HttpPatch("update/status")]
        public async Task<ObjectResult> UpdateOrderByStatus([FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            var command = new UpdateOrderStatusCommand(
                updateOrderStatusDto.Id,
                updateOrderStatusDto.Status
            );

            var handler =
                new ExceptionCatcher<UpdateOrderStatusCommand, UpdateOrderStatusResponse>(
                    new PerfomanceMonitor<UpdateOrderStatusCommand, UpdateOrderStatusResponse>(
                        new LoggingAspect<UpdateOrderStatusCommand, UpdateOrderStatusResponse>(
                            new UpdateOrderStatusCommandHandler(
                                _eventStore,
                                _orderRepository,
                                _messageBrokerService,
                                _publishEndpoint
                            ), _logger
                        ), _logger, _performanceLogsRepository, nameof(UpdateOrderStatusCommandHandler), "Write"
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);

            return Ok(res.Unwrap());
        }

        [HttpPatch("assign/tow")]
        public async Task<ObjectResult> AssignTowDriver([FromBody] AssignTowDriverDto assignTowDriverDto)
        {
            var locationQuery = new FindTowDriverByStatusQuery();
            var result = await locationQuery.Execute();
            var towDrivers = result.Unwrap();
            var devicesQuery = new FindTowDriversDeviceTokenQuery();
            var devicesId = await devicesQuery.Execute(towDrivers);
            var command = new AssignTowDriverCommand(
                assignTowDriverDto.OrderId,
                towDrivers.ToDictionary(towDriver => towDriver.TowDriverId, towDriver => towDriver.Location!),
                devicesId.Unwrap()
            );

            var handler =
                new ExceptionCatcher<AssignTowDriverCommand, AssignTowDriverResponse>(
                    new PerfomanceMonitor<AssignTowDriverCommand, AssignTowDriverResponse>(
                        new LoggingAspect<AssignTowDriverCommand, AssignTowDriverResponse>(
                            new AssignTowDriverCommandHandler(
                                _eventStore,
                                _orderRepository,
                                _publishEndpoint,
                                _messageBrokerService,
                                _locationService,
                                _sagaStateMachineService,
                                _notificationService
                            ), _logger
                        ), _logger, _performanceLogsRepository, nameof(AssignTowDriverCommandHandler), "Write"
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);
            return Ok(res.Unwrap()); 
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPatch("driver/response")]
        public async Task<ObjectResult> DriverResponse([FromBody] TowDriverResponseDto towDriverResponseDto)
        {
            var command = new TowDriverResponseCommand(
                towDriverResponseDto.OrderId,
                towDriverResponseDto.Status,
                towDriverResponseDto.Response
            );

            var handler =
              new ExceptionCatcher<TowDriverResponseCommand, TowDriverResponseResponse>(
                  new PerfomanceMonitor<TowDriverResponseCommand, TowDriverResponseResponse>(
                      new LoggingAspect<TowDriverResponseCommand, TowDriverResponseResponse>(
                          new TowDriverResponseCommandHandler(
                              _eventStore,
                              _orderRepository,
                              _publishEndpoint,
                              _messageBrokerService,
                              _notificationService
                          ), _logger
                      ), _logger, _performanceLogsRepository, nameof(AssignTowDriverCommandHandler), "Write"
                  ), ExceptionParser.Parse
              );
            var res = await handler.Execute(command);
            return Ok(res.Unwrap());
        }

        [HttpPatch("calculate/total")]
        public async Task<ObjectResult> CalculateOrderTotalCost([FromBody] CalculateOrderTotalCostDto calculateOrderTotalCostDto) 
        {
            Console.WriteLine("Holaa1");
            var query = new FindClientPolicyQuery();
            var result = await query.Execute(calculateOrderTotalCostDto.OrderId);
            var clientPolicy = result.Unwrap();
            Console.WriteLine("Holaa1");
            var command = new CalculateOrderTotalCostCommand(
                calculateOrderTotalCostDto.OrderId,
                clientPolicy.coverageAmount,
                0
            );

            var handler =
                new ExceptionCatcher<CalculateOrderTotalCostCommand, CalculateOrderTotalCostResponse>(
                    new PerfomanceMonitor<CalculateOrderTotalCostCommand, CalculateOrderTotalCostResponse>(
                        new LoggingAspect<CalculateOrderTotalCostCommand, CalculateOrderTotalCostResponse>(
                            new CalculateOrderTotalCostCommandHandler(
                                _eventStore,
                                _orderRepository,
                                _messageBrokerService,
                                _locationService
                            ), _logger
                        ), _logger, _performanceLogsRepository, nameof(AssignTowDriverCommandHandler), "Write"
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);
            return Ok(res.Unwrap());
        }

        [HttpGet("find/status/{status}")]
        public async Task<ObjectResult> FindOrderByStatus(string status)
        {
            var data = new FindOrderByStatusDto(status);
            var query = new FindOrderByStatusQuery();
            var res = await query.Execute(data);
            return Ok(res.Unwrap());
        }

        [HttpGet("find/{id}")]
        public async Task<ObjectResult> FindOrderAssigned(string id)
        {
            var data = new FindOrderAssignedDto(id);
            var query = new FindOrderByIdQuery();
            var res = await query.Execute(data);
            return Ok(res.Unwrap());
        }
        

        [HttpDelete("delete/additionalcost")]
        public async Task<ObjectResult> RemoveAdditionalCost([FromBody] RemoveAdditionalCostDto removeAdditionalCostDto)
        {
            var command = new RemoveAdditionalCostCommand(
                removeAdditionalCostDto.OrderId,
                removeAdditionalCostDto.AdditionalCostId
            );

            var handler =
                new ExceptionCatcher<RemoveAdditionalCostCommand, RemoveAdditionalCostResponse>(
                    new PerfomanceMonitor<RemoveAdditionalCostCommand, RemoveAdditionalCostResponse>(
                        new LoggingAspect<RemoveAdditionalCostCommand, RemoveAdditionalCostResponse>(
                            new RemoveAdditionalCostCommandHandler(
                                _eventStore,
                                _orderRepository,
                                _messageBrokerService
                            ), _logger
                        ), _logger, _performanceLogsRepository, nameof(RemoveAdditionalCostCommandHandler), "Write"
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);
            
            return Ok(res.Unwrap());
        }


    }
}