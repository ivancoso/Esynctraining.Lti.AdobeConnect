using System;

namespace Esynctraining.Core.Logging
{
    //
    // Summary:
    //     Manages logging.
    //
    // Remarks:
    //     This is a facade for the different logging subsystems. It offers a simplified
    //     interface that follows IOC patterns and a simplified priority/level/severity
    //     abstraction.
    public interface ILogger
    {
        ////
        //// Summary:
        ////     Determines if messages of priority "debug" will be logged.
        //bool IsDebugEnabled { get; }
        ////
        //// Summary:
        ////     Determines if messages of priority "error" will be logged.
        //bool IsErrorEnabled { get; }
        ////
        //// Summary:
        ////     Determines if messages of priority "fatal" will be logged.
        //bool IsFatalEnabled { get; }
        ////
        //// Summary:
        ////     Determines if messages of priority "info" will be logged.
        //bool IsInfoEnabled { get; }
        ////
        //// Summary:
        ////     Determines if messages of priority "warn" will be logged.
        //bool IsWarnEnabled { get; }

        ////
        //// Summary:
        ////     Create a new child logger. The name of the child logger is [current-loggers-name].[passed-in-name]
        ////
        //// Parameters:
        ////   loggerName:
        ////     The Subname of this logger.
        ////
        //// Returns:
        ////     The New ILogger instance.
        ////
        //// Exceptions:
        ////   T:System.ArgumentException:
        ////     If the name has an empty element name.
        //ILogger CreateChildLogger(string loggerName);
        //
        // Summary:
        //     Logs a debug message with lazily constructed message. The message will be constructed
        //     only if the Castle.Core.Logging.ILogger.IsDebugEnabled is true.
        //
        // Parameters:
        //   messageFactory:
        void Debug(Func<string> messageFactory);
        //
        // Summary:
        //     Logs a debug message.
        //
        // Parameters:
        //   message:
        //     The message to log
        void Debug(string message);
        //
        // Summary:
        //     Logs a debug message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   message:
        //     The message to log
        void Debug(string message, Exception exception);
        //
        // Summary:
        //     Logs a debug message.
        //
        // Parameters:
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void DebugFormat(string format, params object[] args);
        //
        // Summary:
        //     Logs a debug message.
        //
        // Parameters:
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void DebugFormat(IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs a debug message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void DebugFormat(Exception exception, string format, params object[] args);
        //
        // Summary:
        //     Logs a debug message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs an error message with lazily constructed message. The message will be constructed
        //     only if the Castle.Core.Logging.ILogger.IsErrorEnabled is true.
        //
        // Parameters:
        //   messageFactory:
        void Error(Func<string> messageFactory);
        //
        // Summary:
        //     Logs an error message.
        //
        // Parameters:
        //   message:
        //     The message to log
        void Error(string message);
        //
        // Summary:
        //     Logs an error message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   message:
        //     The message to log
        void Error(string message, Exception exception);
        //
        // Summary:
        //     Logs an error message.
        //
        // Parameters:
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void ErrorFormat(string format, params object[] args);
        //
        // Summary:
        //     Logs an error message.
        //
        // Parameters:
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs an error message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void ErrorFormat(Exception exception, string format, params object[] args);
        //
        // Summary:
        //     Logs an error message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs a fatal message with lazily constructed message. The message will be constructed
        //     only if the Castle.Core.Logging.ILogger.IsFatalEnabled is true.
        //
        // Parameters:
        //   messageFactory:
        void Fatal(Func<string> messageFactory);
        //
        // Summary:
        //     Logs a fatal message.
        //
        // Parameters:
        //   message:
        //     The message to log
        void Fatal(string message);
        //
        // Summary:
        //     Logs a fatal message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   message:
        //     The message to log
        void Fatal(string message, Exception exception);
        //
        // Summary:
        //     Logs a fatal message.
        //
        // Parameters:
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void FatalFormat(string format, params object[] args);
        //
        // Summary:
        //     Logs a fatal message.
        //
        // Parameters:
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void FatalFormat(IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs a fatal message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void FatalFormat(Exception exception, string format, params object[] args);
        //
        // Summary:
        //     Logs a fatal message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs a info message with lazily constructed message. The message will be constructed
        //     only if the Castle.Core.Logging.ILogger.IsInfoEnabled is true.
        //
        // Parameters:
        //   messageFactory:
        void Info(Func<string> messageFactory);
        //
        // Summary:
        //     Logs an info message.
        //
        // Parameters:
        //   message:
        //     The message to log
        void Info(string message);
        //
        // Summary:
        //     Logs an info message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   message:
        //     The message to log
        void Info(string message, Exception exception);
        //
        // Summary:
        //     Logs an info message.
        //
        // Parameters:
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void InfoFormat(string format, params object[] args);
        //
        // Summary:
        //     Logs an info message.
        //
        // Parameters:
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void InfoFormat(IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs an info message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void InfoFormat(Exception exception, string format, params object[] args);
        //
        // Summary:
        //     Logs an info message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs a warn message with lazily constructed message. The message will be constructed
        //     only if the Castle.Core.Logging.ILogger.IsWarnEnabled is true.
        //
        // Parameters:
        //   messageFactory:
        void Warn(Func<string> messageFactory);
        //
        // Summary:
        //     Logs a warn message.
        //
        // Parameters:
        //   message:
        //     The message to log
        void Warn(string message);
        //
        // Summary:
        //     Logs a warn message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   message:
        //     The message to log
        void Warn(string message, Exception exception);
        //
        // Summary:
        //     Logs a warn message.
        //
        // Parameters:
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void WarnFormat(string format, params object[] args);
        //
        // Summary:
        //     Logs a warn message.
        //
        // Parameters:
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void WarnFormat(IFormatProvider formatProvider, string format, params object[] args);
        //
        // Summary:
        //     Logs a warn message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void WarnFormat(Exception exception, string format, params object[] args);
        //
        // Summary:
        //     Logs a warn message.
        //
        // Parameters:
        //   exception:
        //     The exception to log
        //
        //   formatProvider:
        //     The format provider to use
        //
        //   format:
        //     Format string for the message to log
        //
        //   args:
        //     Format arguments for the message to log
        void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

    }

}
