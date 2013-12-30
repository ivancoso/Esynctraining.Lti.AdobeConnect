namespace EdugameCloud.Core.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Formats;
    using Esynctraining.Core.Domain.Contracts;

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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> DeleteById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuestionDTO> GetById(int id);

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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuestionDTO> GetByUserIdAndSubModuleItemId(int userId, int smiId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuestionDTO> Save(QuestionDTO dto);

        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="questions">
        /// Question list.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuestionDTO> SaveAll(List<QuestionDTO> questions);

        /// <summary>
        /// Reorder questions
        /// </summary>
        /// <param name="questions">Question list with order</param>
        /// <returns>
        /// Service response
        /// </returns>
        [OperationContract]
        ServiceResponse Reorder(List<QuestionOrderDTO> questions);

        /// <summary>
        /// Export questions by SubModule item id.
        /// </summary>
        /// <param name="smiId">SubModule item id.</param>
        /// <param name="questionIds">Question ids.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        [OperationContract]
        ServiceResponse<string> ExportQuestionsBySubModuleItemId(int smiId, List<int> questionIds);

        /// <summary>
        /// Export by SubModule id.
        /// </summary>
        /// <param name="subModuleId">SubModule id.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        [OperationContract]
        ServiceResponse<string> ExportBySubModuleId(int subModuleId);

        /// <summary>
        /// Import questions by SubModule item id.
        /// </summary>
        /// <param name="id">Import id.</param>
        /// <param name="smiId">SubModule item id.</param>
        /// <param name="format">Import format.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        [OperationContract]
        ServiceResponse ImportQuestionsBySubModuleItemId(string id, int smiId, string format);

        /// <summary>
        /// Import entity by SubModule item id.
        /// </summary>
        /// <param name="id">Import id.</param>
        /// <param name="smiId">SubModule item id.</param>
        /// <param name="format">Import format.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        [OperationContract]
        ServiceResponse ImportBySubModuleItemId(string id, int smiId, string format);

        #endregion
    }
}