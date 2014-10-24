namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The Quiz Service interface.
    /// </summary>
    [ServiceContract]
    public interface IQuizService
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The all.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizDTO> GetAll();

        /// <summary>
        /// Get user by subModuleId.
        /// </summary>
        /// <param name="id">
        /// The subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizDTO> GetById(int id);

        /// <summary>
        /// Get user by subModuleId.
        /// </summary>
        /// <param name="smiId">
        /// The smi Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizDTO> GetBySMIId(int smiId);

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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizDTO> GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The get quiz categoriesby user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SMICategoriesFromStoredProcedureDTO> GetQuizCategoriesbyUserId(int userId);

        /// <summary>
        /// The get quiz data by quiz subModuleId.
        /// </summary>
        /// <param name="quizId">
        /// The quiz subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizDataDTO> GetQuizDataByQuizId(int quizId);

        /// <summary>
        /// The get lms quizzes.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms user parameters id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizFromStoredProcedureDTO> GetLmsQuizzes(int userId, int lmsUserParametersId);

        /// <summary>
        /// The get quiz sm items by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SubModuleItemDTO> GetQuizSMItemsByUserId(int userId);


        /// <summary>
        /// The get quizes by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <param name="showLms">
        /// The show lms.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizFromStoredProcedureDTO> GetQuizzesByUserId(int userId, bool? showLms);

        /// <summary>
        /// The get shared quizes by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizFromStoredProcedureDTO> GetSharedQuizzesByUserId(int userId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="appletItemDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizDTO> Save(QuizDTO appletItemDTO);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizDTO> Create(QuizSMIWrapperDTO dto);

        #endregion
    }
}