namespace EdugameCloud.Lti.Extensions
{
    using System;
    using EdugameCloud.Lti.Core.Constants;
    using System.Web;
    
    public static class UrlStringExtension
    {
        public static string RemoveHttpProtocolAndTrailingSlash(this string url)
        {
            if (url != null)
            {
                if (url.StartsWith(HttpScheme.Http, StringComparison.OrdinalIgnoreCase))
                {
                    url = url.Substring(HttpScheme.Http.Length);
                }

                if (url.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase))
                {
                    url = url.Substring(HttpScheme.Https.Length);
                }

                while (url.EndsWith("/"))
                {
                    url = url.Substring(0, url.Length - 1);
                }
            }

            return url;
        }
        
        public static bool StartsWithProtocol(this string url)
        {
            return !string.IsNullOrWhiteSpace(url) && 
                (url.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase) || url.StartsWith(HttpScheme.Http, StringComparison.OrdinalIgnoreCase));
        }
        
        public static bool IsSSL(this string url)
        {
            return !string.IsNullOrWhiteSpace(url) && url.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase);
        }
        
        public static string AddHttpProtocol(this string url, bool useSsl)
        {
            var domain = url.RemoveHttpProtocolAndTrailingSlash();
            if (useSsl)
            {
                return HttpScheme.Https + domain;
            }

            return HttpScheme.Http + domain;
        }

        public static string GetScheme(this HttpRequestBase request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var protoHeader = request.Headers["X-Forwarded-Proto"];
            bool isHttps = (protoHeader != null) && protoHeader.IndexOf("https", StringComparison.OrdinalIgnoreCase) != -1;
            string schema = request.Url.Scheme;
            if (isHttps)
                schema = "https";

            return schema;
        }

        //public static string GetScheme(this HttpRequest request)
        //{
        //    if (request == null)
        //        throw new ArgumentNullException("request");

        //    var protoHeader = request.Headers["X-Forwarded-Proto"];
        //    bool isHttps = (protoHeader != null) && protoHeader.IndexOf("https", StringComparison.OrdinalIgnoreCase) != -1;
        //    string schema = request.Url.Scheme;
        //    if (isHttps)
        //        schema = "https";

        //    return schema;
        //}

    }

}
