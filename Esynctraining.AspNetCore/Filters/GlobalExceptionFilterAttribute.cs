using System;
using Esynctraining.Core;
using Esynctraining.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Esynctraining.AspNetCore.Filters
{
    // https://blog.kloud.com.au/2016/03/23/aspnet-core-tips-and-tricks-global-exception-handling/
    // https://weblog.west-wind.com/posts/2016/oct/16/error-handling-and-exceptionfilter-dependency-injection-for-aspnet-core-apis
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly bool _isDevelopment;


        public GlobalExceptionFilterAttribute(ILoggerFactory loggerFactory, bool isDevelopment)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger("GlobalExceptionFilter");
            _isDevelopment = isDevelopment;
        }


        public override void OnException(ExceptionContext context)
        {
            string message = string.Empty;
            var userMessage = context.Exception as IUserMessageException;

            if (userMessage != null)
            {
                message = context.Exception.Message;
            }
            else
            {
                _logger.LogError("GlobalExceptionFilter. {0}.", context.Exception);

                message = _isDevelopment
                    ? Resources.Messages.ExceptionOccured + context.Exception.ToString()
                    : Resources.Messages.ExceptionMessage;
            }

            context.Result = new ObjectResult(OperationResult.Error(message));
            //context.ExceptionHandled = true;
        }

    }
}
