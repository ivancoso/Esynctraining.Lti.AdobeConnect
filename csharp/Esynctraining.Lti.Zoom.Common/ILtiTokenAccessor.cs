using System;
using Microsoft.AspNetCore.Http;

namespace Esynctraining.Lti.Zoom.Common
{
    public interface ILtiTokenAccessor
    {
        Guid FetchToken(HttpRequest req, out string mode);
    }
}