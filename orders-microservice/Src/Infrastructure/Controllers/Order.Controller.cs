using Application.Core;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using order.Infrastructure.Responses;
using orders_microservice.Application.Commands.RegisterOrder;
using orders_microservice.Application.Commands.RegisterOrder.types;
using orders_microservice.Application.Commands.UpdateOrder;
using orders_microservice.Application.Commands.UpdateOrder.types;
using orders_microservice.Application.Commands.UpdateOrderStatus;
using orders_microservice.Domain.Repositories;
using orders_microservice.Infrastructure.Controllers.Dtos;
using orders_microservice.Infrastructure.queries;
using orders_microservice.Src.Application.Commands.AssignTowDriver;
using orders_microservice.Src.Application.Commands.AssignTowDriver.Types;
using orders_microservice.Utils.Core.Src.Application.LocationService;
using orders_microservice.Utils.Core.Src.Application.SagaStateMachineService;
using System.Text.Json.Nodes;
using orders_microservice.Src.Application.Commands.RemoveAdditionalCost;
using orders_microservice.Src.Application.Commands.RemoveAdditionalCost.Types;
using orders_microservice.Src.Infrastructure.Controllers.Dtos;
using orders_microservice.Utils.Core.Src.Application.NotificationService;



namespace orders_microservice.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController(
        IdService<string> idService,
        IMessageBrokerService messageBrokerService,
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint,
        INotificationService notificationService,
        ILocationService<JsonNode> locationService,
        ISagaStateMachineService<string> sagaStateMachineService
    )
        : ControllerBase
    {
        private readonly IdService<string> _idService = idService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly ILocationService<JsonNode> _locationService = locationService;
        private readonly ISagaStateMachineService<string> _sagaStateMachineService = sagaStateMachineService;
        private readonly INotificationService _notificationService = notificationService;

        [HttpPost("create")]
        public async Task<ObjectResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var command = new RegisterOrderCommand(
                createOrderDto.Status,
                createOrderDto.IssueLocation,
                createOrderDto.Destination,
                createOrderDto.Details,
                createOrderDto.Name,
                createOrderDto.Image,
                createOrderDto.Policy,
                createOrderDto.PhoneNumber

            );
            var handler =
                new ExceptionCatcher<RegisterOrderCommand, RegisterOrderResponse>(
                    new PerfomanceMonitor<RegisterOrderCommand, RegisterOrderResponse>(
                        new RegisterOrderCommandHandler(
                            _idService,
                            _messageBrokerService,
                            _eventStore,
                            _orderRepository,
                            _publishEndpoint
                        )
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
                        x.Amount.Value 
                    )
                ).ToList()
            );
            var handler =
                new ExceptionCatcher<UpdateOrderCommand, UpdateOrderResponse>(
                    new PerfomanceMonitor<UpdateOrderCommand, UpdateOrderResponse>(
                        new UpdateOrderCommandHandler(
                            _idService,
                            _eventStore,
                            _orderRepository,
                            _messageBrokerService,
                            _publishEndpoint
                        )
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
                        new UpdateOrderStatusCommandHandler(
                            _eventStore,
                            _orderRepository,
                            _messageBrokerService,
                            _publishEndpoint
                        )
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);
            return Ok(res.Unwrap());
        }

        [HttpPatch("assign/tow")]
        public async Task<ObjectResult> AssignTowDriver([FromBody] AssignTowDriverDto assignTowDriverDto)
        {
            var command = new AssignTowDriverCommand(
                assignTowDriverDto.OrderId,
                assignTowDriverDto.TowsLocation
            );
            var handler =
                new ExceptionCatcher<AssignTowDriverCommand, AssignTowDriverResponse>(
                    new PerfomanceMonitor<AssignTowDriverCommand, AssignTowDriverResponse>(
                        new AssignTowDriverCommandHandler(
                            _eventStore,
                            _orderRepository,
                            _publishEndpoint,
                            _messageBrokerService,
                            _locationService,
                            _sagaStateMachineService
                        )
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);
            return Ok(res.Unwrap());
        }

        [HttpGet("find/status/{status}")]
        public async Task<ObjectResult> FindOrderByStatus(string status)
        {
            var data = new FindOrderByStatusDto { Status = status };
            var query = new FindOrderByStatusQuery();
            var res = await query.Execute(data);
            return Ok(res.Unwrap());
        }

        [HttpGet("find/{id}")]
        public async Task<ObjectResult> FindOrderAssigned(string id)
        {
            var data = new FindOrderAssignedDto { Id = id };
            var query = new FindOrderAssignedQuery();
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
                        new RemoveAdditionalCostCommandHandler(
                            _eventStore,
                            _orderRepository,
                            _messageBrokerService
                        )
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);
            return Ok(res.Unwrap());
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification(string deviceToken, string title, string body)
        {
            try
            {
                await _notificationService.SendNotification(deviceToken, title, body);
                return Ok("Notification sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending notification: {ex.Message}");
            }
        }
    }
}