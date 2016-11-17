/*
Copyright (c) 2007-2009 Dmitry Stroganov (DmitryStroganov.info)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

#define TRACE

namespace Esynctraining.AC.Provider
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// The trace tool.
    /// </summary>
    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
    internal static class TraceTool
    {
        /// <summary>
        /// The repeat limit.
        /// </summary>
        private const short RepeatLimit = 4;

        /// <summary>
        /// The m_ stack trace.
        /// </summary>
        private static readonly StringBuilder StackTrace = new StringBuilder();


        private static readonly object Locker = new object();

        /// <summary>
        /// The count.
        /// </summary>
        private static short count = 0;

        /// <summary>
        /// The trace message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void TraceMessage(string message)
        {
            string className = null;
            string methodName = null;

            var stackTrace = new StackTrace();

            if (stackTrace.FrameCount > 1)
            {
                try
                {
                    var stackFrame = stackTrace.GetFrame(1);
                    var methodBase = stackFrame.GetMethod();
                    methodName = methodBase.Name;
                    className = methodBase.ReflectedType.Name;
                }
                catch
                {
                }
            }

            if (string.IsNullOrEmpty(className))
            {
                className = "_UnknownClass";
            }

            if (string.IsNullOrEmpty(methodName))
            {
                methodName = "_UnknownMethod";
            }

            Trace.WriteLine(string.Format("[{0}] at '{1}.{2}': {3}", DateTime.Now.ToString("g"), className, methodName, message));
            Trace.Flush();
        }

        /// <summary>
        /// The trace exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public static void TraceException(Exception ex)
        {
            string className = null;
            string methodName = null;
            string srcLoc = null;

            var stackTrace = new StackTrace();
            if (stackTrace.FrameCount > 1)
            {
                try
                {
                    var stackFrame = stackTrace.GetFrame(1);
                    var fileName = stackFrame.GetFileName();
                    var lineNumber = stackFrame.GetFileLineNumber();
                    var columnNumber = stackFrame.GetFileColumnNumber();

                    srcLoc = (!string.IsNullOrEmpty(fileName))
                                 ? string.Format("'{0}', position [{1},{2}]", fileName, lineNumber, columnNumber)
                                 : null;

                    var methodBase = stackFrame.GetMethod();
                    methodName = methodBase.Name;
                    className = methodBase.ReflectedType.Name;
                }
                catch
                {
                    
                }
            }

            if (string.IsNullOrEmpty(className))
            {
                className = "_UnknownClass";
            }

            if (string.IsNullOrEmpty(methodName))
            {
                methodName = "_UnknownMethod";
            }

            var exceptionText = new StringBuilder();

            exceptionText.AppendFormat("\n[{0}] at '{1}.{2}'", DateTime.Now.ToString("g"), className, methodName);

            if (!string.IsNullOrEmpty(srcLoc))
            {
                exceptionText.AppendLine(srcLoc);
            }

            exceptionText.Append(GetExceptionInfo(ex));
            exceptionText.AppendLine("[-----]");

            Trace.WriteLine(exceptionText.ToString());
            Trace.Flush();
        }

        /// <summary>
        /// The get exception info.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetExceptionInfo(Exception ex)
        {
            lock (Locker)
            {
                var exceptionStack = new StringBuilder();

                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    exceptionStack.AppendFormat("{0}\n{1}\n", ex.Message, ex.StackTrace);
                }
                else
                {
                    exceptionStack.AppendFormat("{0}\n", ex.Message);
                }

                if (ex.InnerException != null)
                {
                    RetriveStackRecursively(ex.InnerException);
                }


                exceptionStack.Append(StackTrace);

                return exceptionStack.ToString();
            }
        }

        /// <summary>
        /// The retrive stack recursively.
        /// </summary>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        private static void RetriveStackRecursively(Exception innerException)
        {
            if (!string.IsNullOrEmpty(innerException.StackTrace))
            {
                StackTrace.AppendFormat("{0}\n", innerException.StackTrace);
            }

            if (count > RepeatLimit)
            {
                return;
            }

            count++;

            if (innerException.InnerException != null)
            {
                RetriveStackRecursively(innerException.InnerException);
            }
        }
    }
}
