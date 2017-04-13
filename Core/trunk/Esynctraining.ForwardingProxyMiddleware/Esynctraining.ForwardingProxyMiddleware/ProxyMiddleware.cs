using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Esynctraining.ForwardingProxyMiddleware
{
    public class HttpMethodParam
    {
        public string Method { get; set; }
    }

    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly ProxyOptions _options;

        private static readonly string XHTTPMethodOverride = "X-HTTP-Method-Override";

        private readonly string _hostWithPort;

        public ProxyMiddleware(RequestDelegate next, IOptions<ProxyOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;

            if (string.IsNullOrEmpty(_options.Host))
            {
                throw new ArgumentException("Options parameter must specify host.", nameof(options));
            }

            // Setting default Port and Scheme if not specified
            if (string.IsNullOrEmpty(_options.Port))
            {
                if (string.Equals(_options.Scheme, "https", StringComparison.OrdinalIgnoreCase))
                {
                    _options.Port = "443";
                }
                else
                {
                    _options.Port = "80";
                }

            }

            if (string.IsNullOrEmpty(_options.Scheme))
            {
                _options.Scheme = "http";
            }

            _hostWithPort = _options.Host + ":" + _options.Port;

            _httpClient = new HttpClient(new HttpClientHandler());
        }

        public Task Invoke(HttpContext context) => HandleHttpRequest(context);

        private async Task HandleHttpRequest(HttpContext context)
        {
            var requestMessage = new HttpRequestMessage();
            var requestMethod = context.Request.Method;

            var methodHeader = context.Request.Headers.FirstOrDefault(x => XHTTPMethodOverride.Equals(x.Key, StringComparison.OrdinalIgnoreCase)).Value;
            if (methodHeader != Microsoft.Extensions.Primitives.StringValues.Empty)
            {
                requestMethod = methodHeader;
            }

            var httpMethod = new HttpMethod(requestMethod);

            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in context.Request.Headers.Where(x => !XHTTPMethodOverride.Equals(x.Key, StringComparison.OrdinalIgnoreCase)))
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            requestMessage.Headers.Host = _hostWithPort;
            //var uriString = $"{_options.Scheme}://{_options.Host}:{_options.Port}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
            var uriString = $"{_options.Scheme}://{_options.Host}:{_options.Port}{context.Request.Path}{context.Request.QueryString}";
            requestMessage.RequestUri = new Uri(uriString);

            requestMessage.Method = httpMethod;
            using (var responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                foreach (var header in responseMessage.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                foreach (var header in responseMessage.Content.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
                context.Response.Headers.Remove("transfer-encoding");
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
        }
    }
}
