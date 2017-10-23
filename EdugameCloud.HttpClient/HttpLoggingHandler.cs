using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EdugameCloud.HttpClient
{
    public class HttpLoggingHandler : DelegatingHandler
    {

        private ILogger _logger;
        private ILogger Logger
        {
            get { return _logger ?? (_logger = IoC.Resolve<ILogger>()) ; }
        }

        public HttpLoggingHandler(HttpMessageHandler innerHandler)
        : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                return response;

            Logger.ErrorFormat("Request: {0}", request.ToString());
            if (request.Content != null)
            {
                Logger.Error(await request.Content.ReadAsStringAsync());
            }

            Logger.ErrorFormat("Response: {0}", response.ToString());
            if (response.Content != null)
            {
                Logger.Error(await response.Content.ReadAsStringAsync());
            }

            return response;
        }
    }
}
