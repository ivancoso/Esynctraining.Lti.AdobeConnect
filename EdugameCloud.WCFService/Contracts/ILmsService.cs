namespace EdugameCloud.WCFService.Contracts
{
    using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Web;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The LMS Service interface.
    /// </summary>
    [ServiceContract]
    public interface ILmsService
    {
        /// <summary>
        /// The get providers.
        /// </summary>
        /// <returns>
        /// The <see cref="LmsProviderDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetProviders", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LmsProviderDTO[] GetProviders();

        /// <summary>
        /// The get quizzes for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS User Parameters Id.
        /// </param>
        /// <returns>
        /// The <see cref="LmsQuizInfoDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizzesForUser?userId={userId}&lmsUserParametersId={lmsUserParametersId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LmsQuizInfoDTO[] GetQuizzesForUser(int userId, int lmsUserParametersId);

        /// <summary>
        /// The get authentication parameters by id.
        /// </summary>
        /// <param name="acId">
        /// The AC id.
        /// </param>
        /// <param name="acDomain">
        /// The AC domain.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserParametersDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here."), OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAuthenticationParametersById?acId={acId}&acDomain={acDomain}&scoId={scoId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LmsUserParametersDTO GetAuthenticationParametersById(string acId, string acDomain, string scoId);

        /// <summary>
        /// The convert quizzes.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS User Parameters Id.
        /// </param>
        /// <param name="quizIds">
        /// The quiz Identities.
        /// </param>
        /// <returns>
        /// The <see cref="QuizesAndSubModuleItemsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "ConvertQuizzes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        QuizesAndSubModuleItemsDTO ConvertQuizzes(int userId, int lmsUserParametersId, int[] quizIds);

        /// <summary>
        /// The get surveys for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS user parameters id.
        /// </param>
        /// <returns>
        /// The <see cref="LmsQuizInfoDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSurveysForUser?userId={userId}&lmsUserParametersId={lmsUserParametersId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LmsQuizInfoDTO[] GetSurveysForUser(int userId, int lmsUserParametersId);

        /// <summary>
        /// The convert surveys.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS user parameters id.
        /// </param>
        /// <param name="quizIds">
        /// The quiz ids.
        /// </param>
        /// <returns>
        /// The <see cref="SurveysAndSubModuleItemsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "ConvertSurveys", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        SurveysAndSubModuleItemsDTO ConvertSurveys(int userId, int lmsUserParametersId, int[] quizIds);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "GetMeetingHostReport", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        PrincipalReportDto[] GetMeetingHostReport(int lmsCompanyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "DeletePrincipals", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        OperationResultDto DeletePrincipals(int lmsCompanyId, string[] principalIds);

    }

}
