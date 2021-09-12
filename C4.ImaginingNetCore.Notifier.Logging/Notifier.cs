using C4.ImaginingNetCore.Notifier.Logging.Response;
using Microsoft.Extensions.Logging;
using System;

namespace C4.ImaginingNetCore.Notifier.Logging
{
    /// <summary>
    /// Log messages are not guaranteed to get sent (success depends of a number of factors)
    /// So, that why I'm using try - catch blocks in eveery sending funcion.
    /// </summary>
    public class Notifier : INotifier
    {
        private readonly ILogger _logger;

        //standar way to capture the log category
        //private readonly ILogger<Notifier> _logger;

        //public Notifier(ILogger<Notifier> logger)
        //{
        //    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //}

        public Notifier(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger("Carlos_ImageDaemon") ?? throw new ArgumentNullException(nameof(factory));
        }

        public Status SendCritical(string message, int eventId = 1000001)
        {
            try
            {
                _logger.LogCritical(message, eventId);
                return new SuccessResponse();
            }
            catch
            {
                return new ErrorResponse();
            }

        }

        public Status SendError(string message, int eventId = 1000001)
        {
            try
            {
                _logger.LogError(message, eventId);
                return new SuccessResponse();
            }
            catch
            {
                return new ErrorResponse();
            }

        }

        public Status SendNotification(string message, int eventId = 1000001)
        {
            try
            {
                _logger.LogInformation(eventId, message );
                return new SuccessResponse();
            }
            catch
            {
                return new ErrorResponse();
            }

        }

        public Status SendTrace(string message, int eventId = 1000001)
        {
            try
            {
                _logger.LogTrace(message, eventId);
                return new SuccessResponse();
            }
            catch
            {
                return new ErrorResponse();
            }

        }

        public Status SendWarning(string message, int eventId = 1000001)
        {
            try
            {
                _logger.LogWarning(message, eventId);
                return new SuccessResponse();
            }
            catch
            {
                return new ErrorResponse();
            }

        }
    }
}
