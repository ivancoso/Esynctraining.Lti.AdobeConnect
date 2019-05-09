using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;

namespace Esynctraining.Lti.Lms.Common.HttpClient
{
    public class HttpMsLoggingFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly ILogger _logger;

        public HttpMsLoggingFilter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            return (builder) =>
            {
                next(builder);
                builder.AdditionalHandlers.Insert(0, new HttpMsLoggingHandler(_logger));
            };
        }
    }
}
