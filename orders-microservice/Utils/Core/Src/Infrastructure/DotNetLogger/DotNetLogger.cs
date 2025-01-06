namespace Application.Core
{
    public class DotNetLogger : Logger
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public DotNetLogger()
        {
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            _logger = _loggerFactory.CreateLogger<DotNetLogger>();
        }

        public void Log(string message)
        {
            _logger.LogInformation(message);
        }
    }
}