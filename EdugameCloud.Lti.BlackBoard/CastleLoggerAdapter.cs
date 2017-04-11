using System;
using Esynctraining.BlackBoardClient;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.BlackBoard
{
    internal sealed class CastleLoggerAdapter : ILog
    {
        private readonly ILogger _logger;

        public CastleLoggerAdapter(ILogger logger)
        {
            _logger = logger;
        }


        public void Error(Exception exception)
        {
            _logger.Error("Error occured in " + this.GetType().FullName, exception);
        }

    }

}
