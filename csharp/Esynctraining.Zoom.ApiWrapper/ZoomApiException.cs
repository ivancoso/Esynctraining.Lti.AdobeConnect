using System;

namespace Esynctraining.Zoom.ApiWrapper
{
    public class ZoomApiException : Exception
    {
        //public IRestResponse Response { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusDescription { get; set; }
        public string Content { get; set; }
    }

    public enum ZoomApiErrorCodes
    {
        UserNotBelongToAccount = 1010,
        UserNotFound = 1001,
        MeetingNotFound = 3001
    }
}