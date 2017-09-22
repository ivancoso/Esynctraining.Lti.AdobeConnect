namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The Quiz Service interface.
    /// </summary>
    [ServiceContract]
    public interface IQuizService
    {
        /// <summary>
        /// Get user by subModuleId.
        /// </summary>
        /// <param name="id">
        /// The subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizDTO GetById(int id);

        /// <summary>
        /// Get user by subModuleId.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetBySMIId?smiId={smiId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizDTO GetBySMIId(int smiId);

        /// <summary>
        /// The get quiz categories by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="SMICategoriesFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizCategoriesbyUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SMICategoriesFromStoredProcedureDTO[] GetQuizCategoriesbyUserId(int userId);

        /// <summary>
        /// The get quiz data by quiz subModuleId.
        /// </summary>
        /// <param name="quizId">
        /// The quiz subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDataDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizDataByQuizId?quizId={quizId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizDataDTO GetQuizDataByQuizId(int quizId);

        /// <summary>
        /// The get LMS quizzes.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS user parameters id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetLmsQuizzes?userId={userId}&lmsUserParametersId={lmsUserParametersId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizFromStoredProcedureDTO[] GetLmsQuizzes(int userId, int lmsUserParametersId);

        /// <summary>
        /// The get quiz sub modules items by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizSMItemsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO[] GetQuizSMItemsByUserId(int userId);

        /// <summary>
        /// The get quizzes by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <param name="showLms">
        /// The show LMS.
        /// </param>
        /// <returns>
        /// The <see cref="QuizFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizzesByUserId?userId={userId}&showLms={showLms}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizFromStoredProcedureDTO[] GetQuizzesByUserId(int userId, bool? showLms);

        /// <summary>
        /// The get shared quizzes by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="QuizFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSharedQuizzesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizFromStoredProcedureDTO[] GetSharedQuizzesByUserId(int userId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="appletItemDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        QuizDTO Save(QuizDTO appletItemDTO);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Create", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        QuizDTO Create(QuizSMIWrapperDTO dto);

    }

}