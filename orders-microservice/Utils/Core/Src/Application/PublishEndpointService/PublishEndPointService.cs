namespace Application.Core
{
    public interface IPublishEndPointService
    {
        Task Publish<T>(T eventMessage) where T : class;
    }

}
