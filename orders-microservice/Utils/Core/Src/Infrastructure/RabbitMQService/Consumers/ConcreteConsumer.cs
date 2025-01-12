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
                Console.WriteLine($"Hola2");
                var orderStatus = JsonSerializer.Deserialize<DriverResponse>(
                    context.Message.Context.ToString()!
                );
                Console.WriteLine($"Hola3");
                if (orderStatus != null)
                {
                    var message = new DriverResponse(
                        context.Message.PublisherId,
                        orderStatus.Status
                    );
                    Console.WriteLine($"Hola4");
                    new MessageProcessor(_serviceProvider).ProcessMessage(message);
                }
            }
            return Task.CompletedTask;
        }
    }
}
