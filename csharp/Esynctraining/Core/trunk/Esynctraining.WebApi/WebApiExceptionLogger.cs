using System.Web.Http.ExceptionHandling;
using Esynctraining.Core.Logging;

namespace Esynctraining.WebApi
{
    //global exception logging for WebApi
    //to use, add the following to WebApiConfig.Register: 
    //config.Services.Add(typeof(IExceptionLogger),new WebApiExceptionLogger(IoC.Resolve<ILogger>()));
    public class WebApiExceptionLogger : ExceptionLogger
    {
        private readonly ILogger _logger;

        public WebApiExceptionLogger(ILogger logger)
        {
            _logger = logger;
        }

        public override void Log(ExceptionLoggerContext context)
        {
            _logger.Error($"Unhandled exception processing {context.Request.Method.ToString()}  {context.Request.RequestUri.ToString()}",
                context.Exception);
        }
    }
}