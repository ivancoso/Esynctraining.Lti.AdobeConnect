using System;
using Castle.Core.Logging;

namespace Esynctraining.AdobeConnect.Tests
{
    public class FakeLogger : Esynctraining.Core.Logging.ILogger
    {
        public ILogger CreateChildLogger(string loggerName)
        {
            return null;
        }

        public void Debug(string message)
        {
            
        }

        public void Debug(Func<string> messageFactory)
        {
            
        }

        public void Debug(string message, Exception exception)
        {
            
        }

        public void DebugFormat(string format, params object[] args)
        {
            
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void Error(string message)
        {
            
        }

        public void Error(Func<string> messageFactory)
        {
            
        }

        public void Error(string message, Exception exception)
        {
            
        }

        public void ErrorFormat(string format, params object[] args)
        {
            
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void Fatal(string message)
        {
            
        }

        public void Fatal(Func<string> messageFactory)
        {
            
        }

        public void Fatal(string message, Exception exception)
        {
            
        }

        public void FatalFormat(string format, params object[] args)
        {
            
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void Info(string message)
        {
            
        }

        public void Info(Func<string> messageFactory)
        {
            
        }

        public void Info(string message, Exception exception)
        {
            
        }

        public void InfoFormat(string format, params object[] args)
        {
            
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void Warn(string message)
        {
            
        }

        public void Warn(Func<string> messageFactory)
        {
            
        }

        public void Warn(string message, Exception exception)
        {
            
        }

        public void WarnFormat(string format, params object[] args)
        {
            
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            
        }

        public bool IsDebugEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsWarnEnabled { get; }
    }
}