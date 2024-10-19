using System.Diagnostics;

namespace Application.Core
{
    public class PerfomanceMonitor<T, U>(IService<T, U> service) : IService<T, U>
    {
        private readonly IService<T, U> _service = service;
        async public Task<Result<U>> Execute(T data)
        {
            var sw = Stopwatch.StartNew();
            var result = await _service.Execute(data);
            sw.Stop();
            Console.WriteLine($"Execution time: {sw.ElapsedMilliseconds}ms");

            return result;
        }
    }
}