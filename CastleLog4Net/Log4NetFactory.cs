using System;
using System.IO;
using Castle.Core.Logging;
using log4net;
using log4net.Config;

namespace Esynctraining.CastleLog4Net
{
    internal sealed class Log4NetFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        internal const string DefaultConfigFileName = "log4net.cfg.xml";


        public Log4NetFactory()
            : this(DefaultConfigFileName)
        {
        }


        public Log4NetFactory(string configFile)
        {
            var file = GetConfigFile(configFile);
            XmlConfigurator.ConfigureAndWatch(file);
        }

        public Log4NetFactory(bool configuredExternally)
        {
            if (configuredExternally)
            {
                return;
            }

            var file = GetConfigFile(DefaultConfigFileName);
            XmlConfigurator.ConfigureAndWatch(file);
        }

        public Log4NetFactory(Stream config)
        {
            XmlConfigurator.Configure(config);
        }

        public override ILogger Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            var log = LogManager.GetLogger(type);
            return new Log4netLogger(log, this);
        }

        public override ILogger Create(Type type, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }

        public override ILogger Create(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            var log = LogManager.GetLogger(name);
            return new Log4netLogger(log, this);
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }

    }

}
