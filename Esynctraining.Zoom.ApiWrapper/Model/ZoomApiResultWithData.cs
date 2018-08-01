namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomApiResultWithData<T> : ZoomApiResult
    { 
        public T Data { get; set; }

        public static ZoomApiResultWithData<T> Error(string errorMessage)
        {
            var operationResultWithData = new ZoomApiResultWithData<T>
            {
                IsSuccess = false,
                Message = errorMessage
            };
            return operationResultWithData;
        }

        public static ZoomApiResultWithData<T> Success(T data)
        {
            var operationResultWithData = new ZoomApiResultWithData<T>
            {
                IsSuccess = true,
                Message = string.Empty,
                Data = data
            };
            return operationResultWithData;
        }

        public static ZoomApiResultWithData<T> Success(string message, T data)
        {
            var operationResultWithData = new ZoomApiResultWithData<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
            return operationResultWithData;
        }
    }
}
