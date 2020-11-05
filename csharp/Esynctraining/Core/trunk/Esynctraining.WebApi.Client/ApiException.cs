using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Esynctraining.WebApi.Client
{
    public class ApiException : Exception
    {
        public HttpResponseMessage Response { get; private set; }

        public HttpStatusCode StatusCode
        {
            get { return this.Response.StatusCode; }
        }

        public IEnumerable<string> Errors
        {
            get { return Data.Values.Cast<string>().ToList(); }
        }

        public ApiError ErrorDetails { get; private set; }


        internal ApiException(HttpResponseMessage response, ApiError deserializedErrorObject)
        {
            Response = response;
            ErrorDetails = deserializedErrorObject;
        }

        internal ApiException(string message, HttpResponseMessage response, ApiError deserializedErrorObject) : base(message)
        {
            Response = response;
            ErrorDetails = deserializedErrorObject;
        }

    }

}
