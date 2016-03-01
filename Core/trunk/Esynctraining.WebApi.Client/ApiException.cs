using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Esynctraining.WebApi.Client
{
    public class ApiException : Exception
    {
        public HttpResponseMessage Response { get; set; }

        public HttpStatusCode StatusCode
        {
            get { return this.Response.StatusCode; }
        }

        public IEnumerable<string> Errors
        {
            get { return this.Data.Values.Cast<string>().ToList(); }
        }

        public ApiError ErrorDetails { get; set; }


        internal ApiException(HttpResponseMessage response, ApiError deserializedErrorObject)
        {
            this.Response = response;
            ErrorDetails = deserializedErrorObject;
        }

        internal ApiException(string message, HttpResponseMessage response, ApiError deserializedErrorObject) : base(message)
        {
            this.Response = response;
            ErrorDetails = deserializedErrorObject;
        }

    }

}
