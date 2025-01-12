using MassTransit;
using Order.Infrastructure;
using RabbitMQ.Contracts;
using System.Text.Json;

namespace Application.Core
{
    public class OrderRejectedConsumer(IServiceProvider serviceProvider) : IConsumer<EventType>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        public Task Consume(ConsumeContext<EventType> context)
        {

            if (context.Message.Type == "TowDriverResponse")
            {
                var orderStatus = JsonSerializer.Deserialize<DriverResponse>(
                    context.Message.Context.ToString()!
                );
                if (orderStatus != null)
                {
                    var message = new DriverResponse(
                        context.Message.PublisherId,
                        orderStatus.Status
                    );
                    new MessageProcessor(_serviceProvider).ProcessMessage(message);
                }
            }
            return Task.CompletedTask;
        }
    }
}
