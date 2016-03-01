using System;
using Esynctraining.Core.Logging;

namespace Esynctraining.Core.Logging.CastleLogger
{
    internal abstract class LoggerBase : ILogger
    {
        private readonly Castle.Core.Logging.ILogger _castleLogger;


        public LoggerBase(Castle.Core.Logging.ILogger logger)
        {
            _castleLogger = logger;
        }


        public void Debug(string message)
        {
            _castleLogger.Debug(message);
        }

        public void Debug(Func<string> messageFactory)
        {
            _castleLogger.Debug(messageFactory);
        }

        public void Debug(string message, Exception exception)
        {
            _castleLogger.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _castleLogger.DebugFormat(format, args);
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            _castleLogger.DebugFormat(exception, format, args);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.DebugFormat(formatProvider, format, args);
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.DebugFormat(exception, formatProvider, format, args);
        }

        public void Error(string message)
        {
            _castleLogger.Error(message);
        }

        public void Error(Func<string> messageFactory)
        {
            _castleLogger.Error(messageFactory);
        }

        public void Error(string message, Exception exception)
        {
            _castleLogger.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _castleLogger.ErrorFormat(format, args);
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            _castleLogger.ErrorFormat(exception, format, args);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.ErrorFormat(formatProvider, format, args);
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.ErrorFormat(exception, formatProvider, format, args);
        }

        public void Fatal(string message)
        {
            _castleLogger.Fatal(message);
        }

        public void Fatal(Func<string> messageFactory)
        {
            _castleLogger.Fatal(messageFactory);
        }

        public void Fatal(string message, Exception exception)
        {
            _castleLogger.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _castleLogger.FatalFormat(format, args);
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            _castleLogger.FatalFormat(exception, format, args);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.FatalFormat(formatProvider, format, args);
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.FatalFormat(exception, formatProvider, format, args);
        }

        public void Info(string message)
        {
            _castleLogger.Info(message);
        }

        public void Info(Func<string> messageFactory)
        {
            _castleLogger.Info(messageFactory);
        }

        public void Info(string message, Exception exception)
        {
            _castleLogger.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _castleLogger.InfoFormat(format, args);
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            _castleLogger.InfoFormat(exception, format, args);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.InfoFormat(formatProvider, format, args);
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.InfoFormat(exception, formatProvider, format, args);
        }

        public void Warn(string message)
        {
            _castleLogger.Warn(message);
        }

        public void Warn(Func<string> messageFactory)
        {
            _castleLogger.Warn(messageFactory);
        }

        public void Warn(string message, Exception exception)
        {
            _castleLogger.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _castleLogger.WarnFormat(format, args);
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            _castleLogger.WarnFormat(exception, format, args);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.WarnFormat(formatProvider, format, args);
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            _castleLogger.WarnFormat(exception, formatProvider, format, args);
        }

    }

}
