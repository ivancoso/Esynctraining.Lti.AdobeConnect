using System;
using System.Net;

namespace Esynctraining.Zoom.ApiWrapper
{
    public class ZoomApiException : Exception
    {
        //public IRestResponse Response { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusDescription { get; set; }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}