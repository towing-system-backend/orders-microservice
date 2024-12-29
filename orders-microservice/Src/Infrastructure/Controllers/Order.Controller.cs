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


namespace orders_microservice.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController(
        IdService<string> idService,
        IMessageBrokerService messageBrokerService,
        IEventStore eventStore,
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint
    )
        : ControllerBase
    {
        private readonly IdService<string> _idService = idService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

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
                updateOrderDto.Destination
            );
            var handler =
                new ExceptionCatcher<UpdateOrderCommand, UpdateOrderResponse>(
                    new PerfomanceMonitor<UpdateOrderCommand, UpdateOrderResponse>(
                        new UpdateOrderCommandHandler(
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

        [HttpGet("find/status/{status}")]
        public async Task<ObjectResult> FindOrderByStatus(string status)
        {
            var data = new FindOrderByStatusDto { Status = status };
            var query =
                new ExceptionCatcher<FindOrderByStatusDto, List<FindOrderByStatusResponse>>(
                    new PerfomanceMonitor<FindOrderByStatusDto, List<FindOrderByStatusResponse>>(
                        new FindOrderByStatusQuery()
                    ), ExceptionParser.Parse
                );
            var res = await query.Execute(data);
            return Ok(res.Unwrap());
        }

        [HttpGet("find/{id}")]
        public async Task<ObjectResult> FindOrderAssigned(string id)
        {
            var data = new FindOrderAssignedDto { Id = id };
            var query =
                new ExceptionCatcher<FindOrderAssignedDto, FindOrderAssignedResponse>(
                    new PerfomanceMonitor<FindOrderAssignedDto, FindOrderAssignedResponse>(
                        new FindOrderAssignedQuery()
                    ), ExceptionParser.Parse
                );
            var res = await query.Execute(data);
            return Ok(res.Unwrap());
        }
    }
}