namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    [ServiceContract]
    public interface ILmsService
    {
        [OperationContract]
        void ConvertQuizzes();

        [OperationContract]
        void SaveAnswers();
    }
}
