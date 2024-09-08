namespace Users.Api.Logging
{
    public class LoggerAdapter<TType> : ILoggerAdapter<TType>
    {
        private readonly ILogger<TType> _logger;
        public void LogError(Exception? exception, string? message, params object?[] args)
        {
            _logger.LogError(exception, message, args);
        }

        public void LogInformation(string? message, params object?[] args)
        {
            _logger.LogInformation(message, args);
        }
    }
}
