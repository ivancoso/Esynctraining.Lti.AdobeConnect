namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///  The distractor service interface.
    /// </summary>
    [ServiceContract]
    public interface IDistractorService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="DistractorDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO[] GetAll();

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
        /// The <see cref="DistractorDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO GetById(int id);

        /// <summary>
        /// Get by user id and sub module item id.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="smiId">
        /// The sub module item Id.
        /// </param>
        /// <returns>
        /// The <see cref="DistractorDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO[] GetAllByUserIdAndSubModuleItemId(int userId, int smiId);

        /// <summary>
        /// Get by user id and question id.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="questionId">
        /// The question Id.
        /// </param>
        /// <returns>
        /// The <see cref="DistractorDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO[] GetAllByUserIdAndQuestionId(int userId, int questionId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="DistractorDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO Save(DistractorDTO resultDto);

        #endregion
    }
}