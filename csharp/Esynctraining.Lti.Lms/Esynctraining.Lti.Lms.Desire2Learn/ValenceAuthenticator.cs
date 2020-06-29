using D2L.Extensibility.AuthSdk;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Esynctraining.Lti.Lms.Desire2Learn
{
    public sealed class ValenceAuthenticator : IAuthenticator
    {
        private readonly ID2LUserContext context;

        public ValenceAuthenticator(ID2LUserContext context)
        {
            this.context = context;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            Uri url = client.BuildUri(request);
            string httpMethod = this.AdaptMethod(request.Method);
            string authQueryString = this.CreateAuthQueryString(this.context.CreateAuthenticatedTokens(url, httpMethod));
            if (url.ToString().IndexOf('?') != -1)
            {
                request.Resource = url.PathAndQuery;
                IRestRequest restRequest = request;
                restRequest.Resource = $"{restRequest.Resource}&{authQueryString}";
                request.Parameters.Clear();
            }
            else
            {
                IRestRequest restRequest = request;
                restRequest.Resource = $"{restRequest.Resource}?{authQueryString}";
            }
        }

        private string AdaptMethod(Method m)
        {
            switch (m)
            {
                case Method.GET:
                case Method.POST:
                case Method.PUT:
                case Method.DELETE:
                    return m.ToString();
                default:
                    throw new ArgumentException(nameof(m));
            }
        }

        private string CreateAuthQueryString(IEnumerable<Tuple<string, string>> tokens)
        {
            return string.Join("&", tokens.Select(token => $"{token.Item1}={token.Item2}"));
        }

    }
}
