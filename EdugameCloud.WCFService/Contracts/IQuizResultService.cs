using System;

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
        Task<QuizResultSaveAllDTO> SaveAllAsync(QuizResultDTO[] results);

    }

}