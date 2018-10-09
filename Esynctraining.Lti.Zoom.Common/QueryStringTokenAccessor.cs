using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Esynctraining.Lti.Zoom.Api
{
    public class QueryStringTokenAccessor : ILtiTokenAccessor
    {
        public Guid FetchToken(HttpRequest req, out string mode)
        {
            mode = "";
            Guid result = Guid.Empty;
            if (req.Query.TryGetValue("session", out StringValues token))
            {
                if (Guid.TryParse(token, out Guid uid))
                {
                    result = uid;
                }
            }

            return result;
        }
    }
}