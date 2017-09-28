﻿namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The SurveyResult Service interface.
    /// </summary>
    [ServiceContract]
    public interface ISurveyResultService
    {
        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="results">
        /// The survey results.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        Task<SurveyResultSaveAllDTO> SaveAll(SurveySummaryResultDTO sResult);

    }

}