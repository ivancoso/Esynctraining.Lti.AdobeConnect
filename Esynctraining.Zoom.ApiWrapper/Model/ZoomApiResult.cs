using System;
using System.Collections.Generic;
using System.Text;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomApiResult
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public static ZoomApiResult Error(string errorMessage)
        {
            return new ZoomApiResult()
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }

        public static ZoomApiResult Success()
        {
            return new ZoomApiResult()
            {
                IsSuccess = true,
                Message = string.Empty
            };
        }
    }
}
