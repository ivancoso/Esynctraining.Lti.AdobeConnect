using System.Runtime.Serialization;

namespace EdugameCloud.Lti
{
    //[DataContract]
    public class OperationResult<T>
    {
        //[DataMember(Name = "isSuccess")]
        public bool isSuccess { get; set; }

        //[DataMember(Name = "message")]
        public string message { get; set; }

        //[DataMember(Name = "data")]
        public T data { get; set; }


        public static OperationResult<T> Error(string errorMessage)
        {
            return new OperationResult<T>
            {
                isSuccess = false, 
                message = errorMessage,
            };
        }

        public static OperationResult<T> Success()
        {
            return new OperationResult<T> 
            {
                isSuccess = true,
                message = string.Empty,
            };
        }

        public static OperationResult<T> Success(T data)
        {
            return new OperationResult<T>
            {
                isSuccess = true,
                message = string.Empty,
                data = data,
            };
        }

    }

    //[DataContract]
    public class OperationResult : OperationResult<object>
    {
        public static OperationResult Error(string errorMessage)
        {
            return new OperationResult
            {
                isSuccess = false,
                message = errorMessage,
            };
        }

        public static OperationResult Success()
        {
            return new OperationResult
            {
                isSuccess = true,
                message = string.Empty,
            };
        }

        public static OperationResult Success(object data)
        {
            return new OperationResult
            {
                isSuccess = true,
                message = string.Empty,
                data = data,
            };
        }
    }

}
