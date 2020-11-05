namespace Esynctraining.AC.Provider
{
    using System;

    /// <summary>
    /// The trace tool.
    /// </summary>
    public static class TraceTool
    {
        private static Action<string> traceAction = (str) => {};


        public static void AttachTraceWriter(Action<string> traceMessageWriter)
        {
            if (traceMessageWriter == null)
                throw new ArgumentNullException(nameof(traceMessageWriter));

            traceAction = traceMessageWriter;
        }


        internal static void TraceMessage(string message)
        {
            traceAction($"[{DateTime.Now.ToString("g")}] {message}. StackTrace: {Environment.StackTrace}.");
        }
        
        internal static void TraceException(Exception ex)
        {
            traceAction($"[{DateTime.Now.ToString("g")}] Exception: {ex}");
        }

    }

}
