using Application.Core;
using Moq;
using MassTransit;
using Order.Application;
using Order.Domain;
using Xunit;
using RabbitMQ.Contracts;

namespace Order.Test
{
    public class UpdateOrderCommandHandlerTests
    {
        private readonly Mock<IdService<string>> _idServiceMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly Mock<IPublishEndPointService> _publishEndpointMock;
        private readonly UpdateOrderCommandHandler _updateOrderCommandHandler;

        public UpdateOrderCommandHandlerTests()
        {
            _idServiceMock = new Mock<IdService<string>>();
            _eventStoreMock = new Mock<IEventStore>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _publishEndpointMock = new Mock<IPublishEndPointService>();
            _updateOrderCommandHandler = new UpdateOrderCommandHandler(
                _idServiceMock.Object,
                _eventStoreMock.Object,
                _orderRepositoryMock.Object,
                _messageBrokerServiceMock.Object,
                _publishEndpointMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Update_Order_When_Order_Not_Found()
        {
            // Arrange
            var command = new UpdateOrderCommand(
                "cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd",
                "Completed",
                "b0458aef-64b8-4528-bb8e-938c59c485d3",
                "El Paraiso",
                [
                    new AdditionalCostCommand(
                        "Cambio de caucho",
                        "Refaccion",
                        50
                    )
               ]
            );

            _orderRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.Order>.Empty());

            // Act
            var result = await _updateOrderCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<OrderNotFoundError>(() => result.Unwrap());
            Assert.Equal("No order was found.", exception.Message);

            _orderRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.Order>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
            _publishEndpointMock.Verify(endpoint => endpoint.Publish(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task Should_Update_All_Order_Properties()
        {
            // Arrange
            var command = new UpdateOrderCommand(
                "cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd",
                "Completed",
                "b0458aef-64b8-4528-bb8e-938c59c485d3",
                "Las Mercedes",
                [
                    new AdditionalCostCommand(
                        "Cambio de caucho",
                        "Refaccion",
                        50
                    )
                ]
            );

            var order = Domain.Order.Create(
               new OrderId("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd"),
               new OrderStatus("ToAssign"),
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
               new OrderTotalDistance(0),
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

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("27f2647d-87e1-46b0-a8c9-03fcc735fc70");

            _orderRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.Order>.Of(order));

            // Act
            var result = await _updateOrderCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(command.Id, result.Unwrap().Id);

            _orderRepositoryMock.Verify(repo => repo.Save(It.Is<Domain.Order>(o =>
                    o.GetOrderId().GetValue() == command.Id &&
                    o.GetOrderStatus().GetValue() == command.Status &&
                    o.GetOrderDestinationLocation().GetValue() == command.Destination &&
                    o.GetOrderTowDriverAssigned().GetValue() == command.TowDriverAssigned &&
                    o.GetAdditionalCosts()!.Any(x => x.GetAdditionalCostName().GetValue() == command.AdditionalCosts[0].Name) &&
                    o.GetAdditionalCosts()!.Any(x => x.GetAdditionalCostCategory().GetValue() == command.AdditionalCosts[0].Category) &&
                    o.GetAdditionalCosts()!.Any(x => x.GetAdditionalCostAmount().GetValue() == command.AdditionalCosts[0].Amount)
                )
            ), Times.Once);

            _eventStoreMock.Verify(store => store.AppendEvents(It.Is<List<DomainEvent>>(events => 
                    events.Count == 4 &&
                    events.Any(e => e is OrderStatusUpdatedEvent) &&
                    events.Any(e => e is OrderTowDriverAssignedUpdatedEvent) &&
                    events.Any(e => e is OrderDestinationLocationUpdatedEvent) &&
                    events.Any(e => e is CreateAdditionalCostEvent)
                )
            ), Times.Once);

            _messageBrokerServiceMock.Verify(service => service.Publish(It.Is<List<DomainEvent>>(events =>
                    events.Count == 4 &&
                    events.Any(e => e is OrderStatusUpdatedEvent) &&
                    events.Any(e => e is OrderTowDriverAssignedUpdatedEvent) &&
                    events.Any(e => e is OrderDestinationLocationUpdatedEvent) &&
                    events.Any(e => e is CreateAdditionalCostEvent)
                )
            ), Times.Once);

            _publishEndpointMock.Verify(endpoint => endpoint.Publish(It.IsAny<object>()), Times.Once);
        }
    }
}