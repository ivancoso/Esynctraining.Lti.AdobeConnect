﻿using System;

namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The QuizResult Service interface.
    /// </summary>
    [ServiceContract]
    public interface IQuizResultService
    {
        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="results">
        /// The applet result DTO items.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        Task<OperationResultDto> SaveAll(QuizSummaryResultDTO quizResult);

        [OperationContract]
        [FaultContract(typeof(Error))]
        Task<EventQuizResultDTO> GetByGuid(Guid guid);

        [OperationContract]
        [FaultContract(typeof(Error))]
        Task<EventQuizResultDTO> GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        Task<EventQuizResultDTO> GetLiveQuizResultByPostQuizGuid(Guid guid);
    }

}