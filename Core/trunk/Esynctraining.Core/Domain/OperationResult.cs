using System;
using System.Runtime.Serialization;

namespace Esynctraining.Core.Domain
{
    [DataContract]
    public class OperationResult
    {
        [DataMember(Name = "isSuccess")]
        public bool IsSuccess { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }


        public static OperationResult Error(string errorMessage)
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = errorMessage,
            };
        }

        public static OperationResult Success()
        {
            return new OperationResult
            {
                IsSuccess = true,
                Message = string.Empty,
            };
        }

    }

    [DataContract]
    public class OperationResultWithData<T> : OperationResult
    {
        [DataMember(Name = "data")]
        public T Data { get; set; }


        public static new OperationResultWithData<T> Error(string errorMessage)
        {
            return new OperationResultWithData<T>
            {
                IsSuccess = false,
                Message = errorMessage,
            };
        }

        [Obsolete("Consider ToSuccessResult extension")]
        public static OperationResultWithData<T> Success(T data)
        {
            return new OperationResultWithData<T>
            {
                IsSuccess = true,
                Message = string.Empty,
                Data = data,
            };
        }

        public static OperationResultWithData<T> Success(string message, T data)
        {
            return new OperationResultWithData<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
            };
        }

    }

    public static class OperationResultExtensions
    {
        public static OperationResultWithData<T> ToSuccessResult<T>(this T data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return OperationResultWithData<T>.Success(data);
        }

    }

}
