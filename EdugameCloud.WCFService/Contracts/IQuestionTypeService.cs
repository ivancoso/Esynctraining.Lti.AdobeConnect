namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The AccountService interface.
    /// </summary>
    [ServiceContract]
    public interface IQuestionTypeService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionTypeDTO[] GetAll();

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="PagedQuestionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        PagedQuestionTypeDTO GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionTypeDTO[] GetActiveTypes();

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
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionTypeDTO GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionTypeDTO Save(QuestionTypeDTO user);
        
        #endregion
    }
}