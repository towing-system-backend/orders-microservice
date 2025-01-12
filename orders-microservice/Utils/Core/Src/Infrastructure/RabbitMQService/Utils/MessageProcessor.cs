using Order.Infrastructure;
using RabbitMQ.Contracts;
using System.Reflection;


namespace Application.Core
{
    public class MessageProcessor(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public Task ProcessMessage(IRabbitMQMessage message)
        {
            using var scope = _serviceProvider.CreateScope();
            var controller = scope.ServiceProvider.GetRequiredService<OrderController>();
            Console.WriteLine($"Hola5");
            var method = controller.GetType().GetMethod(
                $"{message.GetType().Name}",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );
            Console.WriteLine($"Hola: {message.GetType().Name}");
            Console.WriteLine($"Hola6");
            if (method == null) return Task.FromResult(false);
            Console.WriteLine($"Hola7");
            var dto = DtoCreator<IRabbitMQMessage, IDto>.GetDto(message);
            Console.WriteLine($"Hola8");
            method.Invoke(controller, new object[] { dto });

            return Task.CompletedTask;
        }
    }
}