using System;
using System.Resources;
using Esynctraining.Core;
using Esynctraining.Core.Domain;
using Microsoft.AspNetCore.Http;
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
        private readonly int _statusCode;

        private readonly string _exceptionOccuredMessageResourceName;
        private readonly string _exceptionMessageResourceName;
        public Type _exceptionMessageResourceType;

        private ResourceManager _resourceMan;
        private ResourceManager ResourceManager
        {
            get
            {
                if (_resourceMan == null)
                {
                    ResourceManager temp = new ResourceManager(_exceptionMessageResourceType.FullName, _exceptionMessageResourceType.Assembly);
                    _resourceMan = temp;
                }
                return _resourceMan;
            }
        }

        private string ExceptionMessage
        {
            get
            {
                return ResourceManager.GetString(_exceptionMessageResourceName);
            }
        }

        private string ExceptionOccuredMessage
        {
            get
            {
                return ResourceManager.GetString(_exceptionOccuredMessageResourceName);
            }
        }

        public GlobalExceptionFilterAttribute(ILoggerFactory loggerFactory, bool isDevelopment, int statusCode = StatusCodes.Status200OK)
            : this(loggerFactory, isDevelopment, typeof(Resources.Messages), "ExceptionOccured", "ExceptionMessage", statusCode)
        {
            
        }

        public GlobalExceptionFilterAttribute(
            ILoggerFactory loggerFactory, 
            bool isDevelopment, 
            Type exceptionMessageResourceType, 
            string exceptionOccuredMessageResourceName,
            string exceptionMessageResourceName,
            int statusCode = StatusCodes.Status200OK)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger("GlobalExceptionFilter");
            _isDevelopment = isDevelopment;
            _statusCode = statusCode;

            _exceptionOccuredMessageResourceName = exceptionOccuredMessageResourceName ?? throw new ArgumentNullException(nameof(exceptionOccuredMessageResourceName));
            _exceptionMessageResourceName = exceptionMessageResourceName ?? throw new ArgumentNullException(nameof(exceptionMessageResourceName));
            _exceptionMessageResourceType = exceptionMessageResourceType ?? throw new ArgumentNullException(nameof(exceptionMessageResourceType));
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
                _logger.LogError(context.Exception, "GlobalExceptionFilter. {0}.", context.Exception.Message);

                message = _isDevelopment
                    ? ExceptionOccuredMessage + context.Exception.ToString()
                    : ExceptionMessage;
            }

            context.Result = new ObjectResult(OperationResult.Error(message))
            {
                StatusCode = _statusCode
            };
            //context.ExceptionHandled = true;
        }

    }
}
