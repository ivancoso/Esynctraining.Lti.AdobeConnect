using Esynctraining.Core.Utils;

namespace EdugameCloud.Core.Logging
{
    internal sealed class Logger : LoggerBase
    {
        public Logger() : base(IoC.Resolve<Castle.Core.Logging.ILoggerFactory>().Create("Default")) { }
        
    }
}
