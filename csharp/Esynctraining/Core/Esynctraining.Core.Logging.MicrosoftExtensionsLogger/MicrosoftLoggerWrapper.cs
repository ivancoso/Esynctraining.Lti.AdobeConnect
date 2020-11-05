using System;
using Microsoft.Extensions.Logging;

namespace Esynctraining.Core.Logging.MicrosoftExtensionsLogger
{
    public class MicrosoftLoggerWrapper : Esynctraining.Core.Logging.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _innerLogger;


        public MicrosoftLoggerWrapper(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _innerLogger = loggerFactory.CreateLogger("Default");
        }


        public void Debug(string message)
        {
            _innerLogger.LogDebug(message);
        }

        public void Debug(Func<string> messageFactory)
        {
            _innerLogger.LogDebug(messageFactory());
        }

        public void Debug(string message, Exception exception)
        {
            _innerLogger.LogDebug(0, exception, message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _innerLogger.LogDebug(format, args);
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            _innerLogger.LogDebug(0, exception, format, args);
        }

        //public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogDebug(string.Format(formatProvider, format, args));
        //}

        //public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogDebug(0, exception, string.Format(formatProvider, format, args));
        //}

        public void Error(string message)
        {
            _innerLogger.LogError(message);
        }

        public void Error(Func<string> messageFactory)
        {
            _innerLogger.LogError(messageFactory());
        }

        public void Error(string message, Exception exception)
        {
            _innerLogger.LogError(0, exception, message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _innerLogger.LogError(format, args);
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            _innerLogger.LogError(0, exception, format, args);
        }

        //public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogError(string.Format(formatProvider, format, args));
        //}

        //public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogError(0, exception, string.Format(formatProvider, format, args));
        //}

        public void Fatal(string message)
        {
            _innerLogger.LogCritical(message);
        }

        public void Fatal(Func<string> messageFactory)
        {
            _innerLogger.LogCritical(messageFactory());
        }

        public void Fatal(string message, Exception exception)
        {
            _innerLogger.LogCritical(0, exception, message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _innerLogger.LogCritical(format, args);
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            _innerLogger.LogCritical(0, exception, format, args);
        }

        //public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogCritical(string.Format(formatProvider, format, args));
        //}

        //public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogCritical(0, exception, string.Format(formatProvider, format, args));
        //}

        public void Info(string message)
        {
            _innerLogger.LogInformation(message);
        }

        public void Info(Func<string> messageFactory)
        {
            _innerLogger.LogInformation(messageFactory());
        }

        public void Info(string message, Exception exception)
        {
            _innerLogger.LogInformation(0, exception, message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _innerLogger.LogInformation(format, args);
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            _innerLogger.LogInformation(0, exception, format, args);
        }

        //public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogInformation(string.Format(formatProvider, format, args));

        //}

        //public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogInformation(0, exception, string.Format(formatProvider, format, args));
        //}

        public void Warn(string message)
        {
            _innerLogger.LogWarning(message);
        }

        public void Warn(Func<string> messageFactory)
        {
            _innerLogger.LogWarning(messageFactory());
        }

        public void Warn(string message, Exception exception)
        {
            _innerLogger.LogWarning(0, exception, message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _innerLogger.LogWarning(format, args);
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            _innerLogger.LogWarning(0, exception, format, args);
        }

        //public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogWarning(string.Format(formatProvider, format, args));
        //}

        //public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        //{
        //    _innerLogger.LogWarning(0, exception, string.Format(formatProvider, format, args));
        //}

    }

}
