using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class OperationResultDto
    {
        [DataMember(Name = "isSuccess")]
        public bool IsSuccess { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }


        public static OperationResultDto Error(string errorMessage)
        {
            return new OperationResultDto
            {
                IsSuccess = false,
                Message = errorMessage,
            };
        }

        public static OperationResultDto Success()
        {
            return new OperationResultDto
            {
                IsSuccess = true,
                Message = string.Empty,
            };
        }

    }

}
