using System.ServiceModel;
using EdugameCloud.Core.Domain.DTO.OfflineQuiz;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.WCFService.Contracts
{
    [ServiceContract]
    public interface IOfflineQuizService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        OfflineQuizDTO GetQuizByKey(string key);

        OfflineQuizResultDTO SendAnswers(OfflineQuizAnswerDTO[] answers);
    }
}