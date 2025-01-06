namespace Application.Core
{
    public class LoggingAspect<T, U>(IService<T, U> service, Logger logger) : IService<T, U>
    {
        private readonly IService<T, U> _service = service;
        private readonly Logger _logger = logger;
        async public Task<Result<U>> Execute(T data)
        {
            _logger.Log($"Attempting to execute {_service.GetType().Name} with data {data}");
            var res = await _service.Execute(data);

            if (res.IsError)
            {
                _logger.Log($"Error executing {_service.GetType().Name} with data {data}");
            }
            else
            {
                _logger.Log($"Successfully executed {_service.GetType().Name} with data {data}");
            }

            return res;
        }
    }
}