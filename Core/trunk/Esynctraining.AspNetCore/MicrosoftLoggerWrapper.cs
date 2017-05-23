﻿using System;
using Microsoft.Extensions.Logging;

namespace Esynctraining.AspNetCore
{
    public class MicrosoftLoggerWrapper : Esynctraining.Core.Logging.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _innerLogger;


        public MicrosoftLoggerWrapper(Microsoft.Extensions.Logging.ILogger innerLogger)
        {
            _innerLogger = innerLogger ?? throw new ArgumentNullException(nameof(innerLogger));
        }


        public void Debug(Func<string> messageFactory)
        {
            _innerLogger.LogDebug(messageFactory());
        }

        public void Debug(string message)
        {
            _innerLogger.LogDebug(message);
        }

        public void Debug(string message, Exception exception)
        {
            _innerLogger.LogDebug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _innerLogger.LogDebug(string.Format(format, args));
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _innerLogger.LogDebug(string.Format(formatProvider, format, args));
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void Error(Func<string> messageFactory)
        {
            _innerLogger.LogError(messageFactory());
        }

        public void Error(string message)
        {
            _innerLogger.LogError(message);
        }

        public void Error(string message, Exception exception)
        {
            _innerLogger.LogError(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _innerLogger.LogError(string.Format(format, args));
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _innerLogger.LogError(string.Format(formatProvider, format, args));
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void Fatal(Func<string> messageFactory)
        {
            _innerLogger.LogCritical(messageFactory());
        }

        public void Fatal(string message)
        {
            _innerLogger.LogCritical(message);
        }

        public void Fatal(string message, Exception exception)
        {
            _innerLogger.LogCritical(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _innerLogger.LogCritical(string.Format(format, args));
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _innerLogger.LogCritical(string.Format(formatProvider, format, args));
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void Info(Func<string> messageFactory)
        {
            _innerLogger.LogInformation(messageFactory());
        }

        public void Info(string message)
        {
            _innerLogger.LogInformation(message);
        }

        public void Info(string message, Exception exception)
        {
            _innerLogger.LogInformation(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _innerLogger.LogInformation(string.Format(format, args));
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _innerLogger.LogInformation(string.Format(formatProvider, format, args));
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void Warn(Func<string> messageFactory)
        {
            _innerLogger.LogWarning(messageFactory());
        }

        public void Warn(string message)
        {
            _innerLogger.LogWarning(message);
        }

        public void Warn(string message, Exception exception)
        {
            _innerLogger.LogWarning(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _innerLogger.LogWarning(string.Format(format, args));
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _innerLogger.LogWarning(string.Format(formatProvider, format, args));
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

    }

}