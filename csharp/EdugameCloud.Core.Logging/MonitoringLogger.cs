using Esynctraining.Core.Utils;

namespace EdugameCloud.Core.Logging
{
    internal sealed class MonitoringLogger : LoggerBase
    {
        public MonitoringLogger() : base(IoC.Resolve<Castle.Core.Logging.ILoggerFactory>().Create("Monitoring")) { }

    }

}
