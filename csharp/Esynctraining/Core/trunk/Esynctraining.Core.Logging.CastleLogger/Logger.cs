using Esynctraining.Core.Utils;

namespace Esynctraining.Core.Logging.CastleLogger
{
    internal sealed class Logger : LoggerBase
    {
        public Logger() : base(IoC.Resolve<Castle.Core.Logging.ILoggerFactory>().Create("Default")) { }

    }

}
