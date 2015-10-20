using System;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.AdobeConnect.Caching
{
    public sealed class ConsoleLog : ILog
    {
        private readonly ILogger _logger;

        public ConsoleLog()
        {
            _logger = IoC.Resolve<ILogger>();
        }


        public void WriteLine(string value)
        {
            Console.WriteLine(value);
            _logger.Info(value);
        }

        public void WriteLine(Exception ex, string tab)
        {
            Console.WriteLine(tab + ex.Message);
            _logger.Error(tab + ex.Message);
        }

    }

}
