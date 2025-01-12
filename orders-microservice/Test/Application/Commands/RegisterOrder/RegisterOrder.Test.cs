using Application.Core;
using MassTransit;
using Moq;
using Order.Application;
using Order.Domain;
using Xunit;

namespace Order.Test
{
    public class RegisterOrderCommandHandlerTests
    {
        private readonly Mock<IdService<string>> _idServiceMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly RegisterOrderCommandHandler _registerOrderCommandHandler;

        public RegisterOrderCommandHandlerTests()
        {
            _idServiceMock = new Mock<IdService<string>>();
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _eventStoreMock = new Mock<IEventStore>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _registerOrderCommandHandler = new RegisterOrderCommandHandler(
                _idServiceMock.Object,
                _messageBrokerServiceMock.Object,
                _eventStoreMock.Object,
                _orderRepositoryMock.Object,
                _publishEndpointMock.Object
            );
        }

        [Fact]
        public async Task Should_Register_Order()
        {
            // Arrange
            var command = new RegisterOrderCommand(
                "ToAssign",
                "Centro Comercial Tolon",
                "El Paraiso",
                "424fe2a9-f91e-4925-8d59-3b16b45cd753",
                "Esta dentro de un estacionamiento, en el centro comercial Tolon, sotano 2.",
                "Juan Hernandez",
                "https://png.pngtree.com/png-clipart/20230927/original/pngtree-man-in-shirt-smiles-and-gives-thumbs-up-to-show-approval-png-image_13146336.png",
                "1ab5ceae-f0f8-4505-b353-a5470a2318fe",
                "04146577845",
                25547458
            );

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd");

            // Act
            var result = await _registerOrderCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd", result.Unwrap().Id);

                _orderRepositoryMock.Verify(repo => repo.Save(It.Is<Domain.Order>(order =>
                   order.GetOrderId().GetValue() == "cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd" &&
                   order.GetOrderStatus().GetValue() == command.Status &&
                   order.GetOrderIssueLocation().GetValue() == command.IssueLocation &&
                   order.GetOrderIssuer().GetValue() == command.Issuer &&
                   order.GetOrderDestinationLocation().GetValue() == command.Destination &&
                   order.GetOrderTowDriverAssigned().GetValue() == "Not Assigned." &&
                   order.GetOrderDetails().GetValue() == command.Details &&
                   order.GetOrderClientInformation().GetClientName() == command.Name &&
                   order.GetOrderClientInformation().GetClientImage() == command.Image &&
                   order.GetOrderClientInformation().GetClientPolicyId() == command.Policy &&
                   order.GetOrderClientInformation().GetClientPhoneNumber() == command.PhoneNumber &&
                   order.GetOrderClientInformation().GetClientIdentificationNumber() == command.IdentificationNumber &&
                   order.GetOrderTotalCost().GetValue() == 0
               )
           ), Times.Once);

            _publishEndpointMock.Verify(endpoint => endpoint.Publish(It.Is<OrderCreatedEventt>(e =>
                e.OrderId == Guid.Parse("cf7e4192-dab5-4c86-a0c6-cc8c597f3dbd")
            ), default), Times.Once);

            _eventStoreMock.Verify(store => store.AppendEvents(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 && events[0] is OrderCreatedEvent
                )
            ), Times.Once);

            _messageBrokerServiceMock.Verify(service => service.Publish(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 && events[0] is OrderCreatedEvent
                )
            ), Times.Once);
        }
    }
}