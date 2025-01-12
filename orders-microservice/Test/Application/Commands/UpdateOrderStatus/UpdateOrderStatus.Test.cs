using Application.Core;
using Moq;
using MassTransit;
using Order.Application;
using Order.Domain;
using Xunit;

namespace Order.Test
{
    public class UpdateOrderStatusCommandHandlerTests
    {
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly UpdateOrderStatusCommandHandler _updateOrderStatusCommandHandler;

        public UpdateOrderStatusCommandHandlerTests()
        {
            _eventStoreMock = new Mock<IEventStore>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _updateOrderStatusCommandHandler = new UpdateOrderStatusCommandHandler(
                _eventStoreMock.Object,
                _orderRepositoryMock.Object,
                _messageBrokerServiceMock.Object,
                _publishEndpointMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Update_Order_Status_When_Order_Not_Found()
        {
            // Arrange
            var command = new UpdateOrderStatusCommand("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd", "Paid");

            _orderRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.Order>.Empty());

            // Act
            var result = await _updateOrderStatusCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<OrderNotFoundError>(() => result.Unwrap());
            Assert.Equal("No order was found.", exception.Message);

            _orderRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.Order>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
            _publishEndpointMock.Verify(endpoint => endpoint.Publish(It.IsAny<object>(), default), Times.Never);
        }

        [Fact]
        public async Task Should_Update_Order()
        {
            // Arrange
            var command = new UpdateOrderStatusCommand("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd", "Paid");

            var order = Domain.Order.Create(
               new OrderId("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd"),
               new OrderStatus("Completed"),
               new OrderIssueLocation("Centro Comercial Tolon"),
               new OrderDestinationLocation("El Paraiso"),
               new OrderTowDriverAssigned("Not assigned"),
               new OrderIssuer("424fe2a9-f91e-4925-8d59-3b16b45cd753"),
               new OrderDetails("Esta dentro de un estacionamiento, en el centro comercial Tolon, sotano 2."),
               new OrderClientInformation(
                   "Juan Hernandez",
                   "https://png.pngtree.com/png-clipart/20230927/original/pngtree-man-in-shirt-smiles-and-gives-thumbs-up-to-show-approval-png-image_13146336.png",
                   "1ab5ceae-f0f8-4505-b353-a5470a2318fe",
                   "04146577845",
                   25547458
               ),
               new OrderTotalCost(0),
               [
                    new AdditionalCost(
                        new AdditionalCostId("28499581-b066-4ad7-b0ad-b6cc266e847e"),
                        new AdditionalCostAmount(1004),
                        new AdditionalCostName("Estacionamiento"),
                        new AdditionalCostCategory("Especial")
                    )
               ],
               true
            );

            _orderRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.Order>.Of(order));

            // Act
            var result = await _updateOrderStatusCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(command.Id, result.Unwrap().Id);

            _orderRepositoryMock.Verify(repo => repo.Save(It.Is<Domain.Order>(o =>
                o.GetOrderId().GetValue() == command.Id &&
                o.GetOrderStatus().GetValue() == "Paid"
            )), Times.Once);

            _eventStoreMock.Verify(store => store.AppendEvents(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 &&
                    events[0] is OrderStatusUpdatedEvent
                )
            ), Times.Once);

            _messageBrokerServiceMock.Verify(service => service.Publish(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 &&
                    events[0] is OrderStatusUpdatedEvent
                )
            ), Times.Once);

            _publishEndpointMock.Verify(endpoint => endpoint.Publish(It.Is<UpdateOrderStatusEvent>(e =>
                e.OrderId == Guid.Parse(command.Id)
            ), default), Times.Once);
        }
    }
}
