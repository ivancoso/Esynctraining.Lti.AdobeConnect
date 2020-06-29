using Esynctraining.Core.Logging;
using Microsoft.Extensions.Http;
using System;

namespace Esynctraining.HttpClient
{
    public class CustomLoggingFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly ILogger _logger;

        public CustomLoggingFilter(ILogger logger)
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
                builder.AdditionalHandlers.Insert(0, new HttpLoggingHandler(_logger));
            };
        }
    }
}
