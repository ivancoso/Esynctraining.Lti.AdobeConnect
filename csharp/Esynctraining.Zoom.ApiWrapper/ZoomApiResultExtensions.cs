using Esynctraining.Zoom.ApiWrapper.Model;
using System;

namespace Esynctraining.Zoom.ApiWrapper
{
    public static class ZoomApiResultExtensions
    {
        public static ZoomApiResultWithData<T> ToSuccessZoomApiResult<T>(this T data)
        {
            if ((object)data == null)
                throw new ArgumentNullException(nameof(data));
            return ZoomApiResultWithData<T>.Success(data);
        }
    }
}
