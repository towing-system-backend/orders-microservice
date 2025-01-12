using Application.Core;
using Moq;
using Order.Application;
using Order.Domain;
using Xunit;

namespace Order.Test
{
    public class RemoveAdditionalCostCommandHandlerTests
    {
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly RemoveAdditionalCostCommandHandler _removeAdditionalCostCommandHandler;

        public RemoveAdditionalCostCommandHandlerTests()
        {
            _eventStoreMock = new Mock<IEventStore>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _removeAdditionalCostCommandHandler = new RemoveAdditionalCostCommandHandler(
                _eventStoreMock.Object,
                _orderRepositoryMock.Object,
                _messageBrokerServiceMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Remove_Additional_Cost_When_Order_Not_Found()
        {
            // Arrange
            var command = new RemoveAdditionalCostCommand("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd", "8b22f5a5-c39a-41e9-b769-cdf23d0d5192");

            _orderRepositoryMock.Setup(repo => repo.FindById(command.OrderId))
                .ReturnsAsync(Optional<Domain.Order>.Empty());

            // Act
            var result = await _removeAdditionalCostCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<OrderNotFoundError>(() => result.Unwrap());
            Assert.Equal("No order was found.", exception.Message);

            _orderRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.Order>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Remove_Additional_Cost_When_Additional_Cost_Not_Found()
        {
            // Arrange
            var command = new RemoveAdditionalCostCommand("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd", "8b22f5a5-c39a-41e9-b769-cdf23d0d5192");

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

            _orderRepositoryMock.Setup(repo => repo.FindById(command.OrderId))
                .ReturnsAsync(Optional<Domain.Order>.Of(order));

            // Act
            var result = await _removeAdditionalCostCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<AdditionalCostNotFoundError>(() => result.Unwrap());
            Assert.Equal("Additional cost not found.", exception.Message);

            _orderRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.Order>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Remove_Additional_Cost()
        {
            // Arrange
            var command = new RemoveAdditionalCostCommand("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd", "62a25158-b2c5-4566-8167-4b51e14b7d61");

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
               [
                    new AdditionalCost(
                        new AdditionalCostId("62a25158-b2c5-4566-8167-4b51e14b7d61"),
                        new AdditionalCostAmount(1004),
                        new AdditionalCostName("Estacionamiento"),
                        new AdditionalCostCategory("Especial")
                    )
               ],
               true
            );

            _orderRepositoryMock.Setup(repo => repo.FindById(command.OrderId))
                .ReturnsAsync(Optional<Domain.Order>.Of(order));

            List<DomainEvent> capturedEvents = new List<DomainEvent>();
            _eventStoreMock.Setup(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()))
                .Callback((List<DomainEvent> events) => capturedEvents.AddRange(events));

            // Act
            var result = await _removeAdditionalCostCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(command.OrderId, result.Unwrap().OrderId);

            _orderRepositoryMock.Verify(repo => repo.Save(It.Is<Domain.Order>(o =>
                    o.GetOrderId().GetValue() == command.OrderId &&
                    !o.GetAdditionalCosts().Any(x => x.GetAdditionalCostId().GetValue() == command.AdditionalCostId)
                )
            ), Times.Once);

            _eventStoreMock.Verify(store => store.AppendEvents(It.Is<List<DomainEvent>>(events => 
                    events.Count == 1 &&
                    events.Any(e => e is AdditionalCostRemovedEvent)
                )
            ), Times.Once);
            
            _messageBrokerServiceMock.Verify(service => service.Publish(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 &&
                    events.Any(e => e is AdditionalCostRemovedEvent)
                )
            ), Times.Once);
        }
    }
}