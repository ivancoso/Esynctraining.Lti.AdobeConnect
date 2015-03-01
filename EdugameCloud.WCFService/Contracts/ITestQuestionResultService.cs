namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The TestQuestionResult Service interface.
    /// </summary>
    [ServiceContract]
    public interface ITestQuestionResultService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="TestQuestionResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestQuestionResultDTO[] GetAll();

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
        /// The <see cref="TestQuestionResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestQuestionResultDTO GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="TestQuestionResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestQuestionResultDTO Save(TestQuestionResultDTO resultDto);

        /// <summary>
        /// The save questions results.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <returns>
        /// The <see cref="TestQuestionResultSaveAllDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestQuestionResultSaveAllDTO SaveAll(TestQuestionResultDTO[] results);

        #endregion
    }
}