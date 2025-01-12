using MassTransit;

namespace Application.Core
{
    public class PublishEndPointService(IPublishEndpoint publishEndpoint) : IPublishEndPointService
    {
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        public async Task Publish<T>(T eventMessage) where T : class
        {
            await _publishEndpoint.Publish(eventMessage);
        }
       
    }
}