﻿using System.ServiceModel;
using System.ServiceModel.Web;
using EdugameCloud.Core.Domain.DTO.OfflineQuiz;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.WCFService.Contracts
{
    [ServiceContract]
    public interface IOfflineQuizService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "Quiz/{key}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        OfflineQuizDTO GetQuizByKey(string key);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "QuizAnswers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [return: MessageParameter(Name = "result")]
        OfflineQuizResultDTO SendAnswers(OfflineQuizAnswerContainerDTO answerContainer);

    }

}