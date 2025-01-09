namespace Application.Core
{
    using System.Diagnostics;
    public class PerfomanceMonitor<T, U>(IService<T, U> service, Logger logger, IPerformanceLogsRepository performanceLogsRepository, string operation, string operationType) : IService<T, U>
    {
        private readonly IService<T, U> _service = service;
        private readonly Logger _logger = logger;
        private readonly IPerformanceLogsRepository _performanceLogsRepository = performanceLogsRepository;
        public async Task<Result<U>> Execute(T data)
        {
            var sw = Stopwatch.StartNew();
            var result = await _service.Execute(data);
            sw.Stop();
            _logger.Log($"Execution time: {sw.ElapsedMilliseconds}ms");
            if (result.IsSuccess)
                await Task.Run(() => _performanceLogsRepository.LogStats(operation, operationType, sw.ElapsedMilliseconds));

            return result;
        }
    }
}