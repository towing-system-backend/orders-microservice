namespace Application.Core
{
    public class LoggingAspect<T, U>(IService<T, U> service) : IService<T, U>
    {
        private readonly IService<T, U> _service = service;
        async public Task<Result<U>> Execute(T data)
        {
            Console.WriteLine($"Attempting to execute {_service.GetType().Name} with data {data}");
            var res = await _service.Execute(data);

            if (res.IsError)
            {
                Console.WriteLine($"Error executing {_service.GetType().Name} with data {data}");
            }
            else
            {
                Console.WriteLine($"Successfully executed {_service.GetType().Name} with data {data}");
            }

            return res;
        }
    }
}