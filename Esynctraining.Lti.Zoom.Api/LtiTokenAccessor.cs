using System;
using Microsoft.AspNetCore.Http;

namespace Esynctraining.Lti.Zoom.Api
{
    public class LtiTokenAccessor : ILtiTokenAccessor
    {
        private static readonly string HeaderName = "Authorization";
        private static readonly string ltiAuthScheme = "lti ";
        private static readonly string apiAuthScheme = "ltiapi ";

        public Guid FetchToken(HttpRequest req, out string mode)
        {
            mode = null;
            string authHeader = req.Headers[HeaderName];

            if ((authHeader != null) && authHeader.StartsWith(ltiAuthScheme, StringComparison.OrdinalIgnoreCase))
            {
                string token = authHeader.Substring(ltiAuthScheme.Length).Trim();

                Guid uid;
                if (Guid.TryParse(token, out uid))
                {
                    mode = ltiAuthScheme;
                    return uid;
                }
            }

            if ((authHeader != null) && authHeader.StartsWith(apiAuthScheme, StringComparison.OrdinalIgnoreCase))
            {
                var parts = authHeader.Substring(apiAuthScheme.Length).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return Guid.Empty;

                string token = parts[0];

                Guid uid;
                if (Guid.TryParse(token, out uid))
                {
                    mode = apiAuthScheme;
                    return uid;
                }
            }

            return Guid.Empty;
        }
    }
}