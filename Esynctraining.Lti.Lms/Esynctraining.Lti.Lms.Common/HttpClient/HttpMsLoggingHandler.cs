using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.Common.HttpClient
{
    public class HttpMsLoggingHandler : DelegatingHandler
    {
        private ILogger _logger;

        public HttpMsLoggingHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                return response;

            _logger.LogError("Request: {0}", request.ToString());
            if (request.Content != null)
            {
                _logger.LogError(await request.Content.ReadAsStringAsync());
            }

            _logger.LogError("Response: {0}", response.ToString());
            if (response.Content != null)
            {
                try
                {
                    //straight call to ReadAsStringAsync can lead to exception (The character set provided in ContentType is invalid) for schoology client
                    var byteContent = await response.Content.ReadAsByteArrayAsync();
                    var stringContent = System.Text.Encoding.UTF8.GetString(byteContent);
                    _logger.LogError(stringContent);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error reading response content", e);
                }
            }

            return response;
        }
    }
}
