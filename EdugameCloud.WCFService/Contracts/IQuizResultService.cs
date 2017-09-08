using System;

namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The QuizResult Service interface.
    /// </summary>
    [ServiceContract]
    public interface IQuizResultService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="QuizResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuizResultDTO[] GetAll();

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuizResultDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuizResultDTO GetByGuid(Guid guid);

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
        QuizResultSaveAllDTO SaveAll(QuizResultDTO[] results);

        #endregion
    }
}