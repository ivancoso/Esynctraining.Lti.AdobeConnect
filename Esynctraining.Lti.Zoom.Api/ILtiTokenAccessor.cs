using System;
using Microsoft.AspNetCore.Http;

namespace Esynctraining.Lti.Zoom.Api
{
    public interface ILtiTokenAccessor
    {
        Guid FetchToken(HttpRequest req, out string mode);
    }

    //public interface ILtiHeaderTokenAccessor : ILtiTokenAccessor { }

}