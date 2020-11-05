using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Esynctraining.Pdf2SwfConverter.Console
{
    public static class SerilogConfigure
    {
        public static ILogger SetupSerilog(this IConfiguration config)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(config).CreateLogger();

            var loggerFactory = new LoggerFactory().AddSerilog(Log.Logger);

            return loggerFactory.CreateLogger("Default");
        }

    }

    
}
