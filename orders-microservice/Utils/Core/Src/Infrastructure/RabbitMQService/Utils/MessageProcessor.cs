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
            var method = controller.GetType().GetMethod(
                $"{message.GetType().Name}",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );
            if (method == null) return Task.FromResult(false);
            var dto = DtoCreator<IRabbitMQMessage, IDto>.GetDto(message);
            method.Invoke(controller, new object[] { dto });

            return Task.CompletedTask;
        }
    }
}