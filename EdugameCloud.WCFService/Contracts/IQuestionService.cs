namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///  The question Service interface.
    /// </summary>
    [ServiceContract]
    public interface IQuestionService
    {
        #region Public Methods and Operators

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
        /// The <see cref="QuestionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO GetById(int id);

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
        /// The <see cref="QuestionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO[] GetByUserIdAndSubModuleItemId(int userId, int smiId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO Save(QuestionDTO dto);

        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="questions">
        /// Question list.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionSaveAllDTO SaveAll(QuestionDTO[] questions);

        /// <summary>
        /// Reorder questions
        /// </summary>
        /// <param name="questions">
        /// Question list with order
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void Reorder(QuestionOrderDTO[] questions);

        /// <summary>
        /// Export questions by SubModule item id.
        /// </summary>
        /// <param name="smiId">SubModule item id.</param>
        /// <param name="questionIds">Question ids.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        string ExportQuestionsBySubModuleItemId(int smiId, int[] questionIds);

        /// <summary>
        /// Export by SubModule id.
        /// </summary>
        /// <param name="subModuleId">SubModule id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        string ExportBySubModuleId(int subModuleId);

        /// <summary>
        /// Import questions by SubModule item id.
        /// </summary>
        /// <param name="id">Import id.</param>
        /// <param name="smiId">SubModule item id.</param>
        /// <param name="format">Import format.</param>
        /// <returns>The <see cref="QuestionDTO"/>.</returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO[] ImportQuestionsBySubModuleItemId(string id, int smiId, string format);

        /// <summary>
        /// The get parsed questions by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO[] GetParsedQuestionsById(string id, int userId, string format);

        #endregion
    }
}