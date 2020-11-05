using System;

namespace Esynctraining.LogViewer.MvcWeb.Models
{
    /// <summary>
    /// Maps to the Log table.
    /// </summary>
    public class Log
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Thread { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }

        public string ShortenedLogger
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Logger))
                {
                    return Logger;
                }

                if (!Logger.Contains("."))
                {
                    return Logger;
                }

                var lastDotPos = Logger.LastIndexOf(".");
                return string.Concat("...", Logger.Substring(lastDotPos + 1, (Logger.Length - lastDotPos) - 1));
            }
        }

    }

}