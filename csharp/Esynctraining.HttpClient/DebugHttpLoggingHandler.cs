using Esynctraining.Core.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Esynctraining.HttpClient
{
    public class DebugHttpLoggingHandler : DelegatingHandler
    {
        private ILogger _logger;

        public DebugHttpLoggingHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            _logger.DebugFormat("Request: {0}", request.ToString());
            if (request.Content != null)
            {
                _logger.Debug(await request.Content.ReadAsStringAsync());
            }

            _logger.DebugFormat("Response: {0}", response.ToString());
            if (response.Content != null)
            {
                try
                {
                    //straight call to ReadAsStringAsync can lead to exception (The character set provided in ContentType is invalid) for schoology client
                    var byteContent = await response.Content.ReadAsByteArrayAsync();
                    var stringContent = System.Text.Encoding.UTF8.GetString(byteContent);
                    _logger.Debug(stringContent);
                }
                catch (Exception e)
                {
                    _logger.Error("Error reading response content", e);
                }
            }

            return response;
        }
    }
}
