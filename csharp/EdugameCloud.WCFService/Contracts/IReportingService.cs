namespace EdugameCloud.WCFService.Contracts
{
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The Reporting Service interface.
    /// </summary>
    [ServiceContract]
    public interface IReportingService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get crossword result by AC session id.
        /// </summary>
        /// <param name="acSessionId">
        /// The AC session id.
        /// </param>
        /// <returns>
        /// The <see cref="CrosswordResultByAcSessionFromStoredProcedureDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here."), OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetCrosswordResultByACSessionId?acSessionId={acSessionId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CrosswordResultByAcSessionDTO[] GetCrosswordResultByACSessionId(int acSessionId);

        /// <summary>
        /// The get crossword sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="CrosswordSessionFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetCrosswordSessionsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CrosswordSessionDTO[] GetCrosswordSessionsByUserId(int userId);

        /// <summary>
        /// The get quiz result by AC session id.
        /// </summary>
        /// <param name="acSessionId">
        /// The AC session id.
        /// </param>
        /// <param name="smiId">
        /// The SMI id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultDataDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed here"), OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizResultByACSessionId?acSessionId={acSessionId}&smiId={smiId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizResultDataDTO GetQuizResultByACSessionId(int acSessionId, int smiId);

        /// <summary>
        /// The get survey result by AC session id.
        /// </summary>
        /// <param name="acSessionId">
        /// The AC session id.
        /// </param>
        /// <param name="smiId">
        /// The SMI id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyResultDataDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed here"), OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSurveyResultByACSessionId?acSessionId={acSessionId}&smiId={smiId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SurveyResultDataDTO GetSurveyResultByACSessionId(int acSessionId, int smiId);

        /// <summary>
        /// The get survey result by AC session id.
        /// </summary>
        /// <param name="acSessionId">
        /// The AC session id.
        /// </param>
        /// <param name="smiId">
        /// The SMI id.
        /// </param>
        /// <returns>
        /// The <see cref="TestResultDataDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed here"), OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetTestResultByACSessionId?acSessionId={acSessionId}&smiId={smiId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        TestResultDataDTO GetTestResultByACSessionId(int acSessionId, int smiId);

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizSessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizSessionsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizSessionDTO[] GetQuizSessionsByUserId(int userId);

        /// <summary>
        /// The get survey sessions by user id.
        /// </summary>
        /// <returns>
        /// The <see cref="SurveySessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "TestDotAMF", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SurveySessionDTO[] TestDotAMF();

        /// <summary>
        /// The get survey sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveySessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSurveySessionsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SurveySessionDTO[] GetSurveySessionsByUserId(int userId);

        /// <summary>
        /// The get test sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="TestSessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetTestSessionsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        TestSessionDTO[] GetTestSessionsByUserId(int userId);

        /// <summary>
        /// The get recent splash screen reports.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="PagedRecentReportsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetRecentSplashScreenReports?userId={userId}&pageIndex={pageIndex}&pageSize={pageSize}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        PagedRecentReportsDTO GetRecentSplashScreenReports(int userId, int pageIndex, int pageSize);

        /// <summary>
        /// The get SN data by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The AC session id.
        /// </param>
        /// <returns>
        /// The <see cref="SNReportingDataDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSNDataByACSessionId?sessionId={sessionId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SNReportingDataDTO GetSNDataByACSessionId(int sessionId);

        /// <summary>
        /// The get SN sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SNSessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSNSessionsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SNSessionDTO[] GetSNSessionsByUserId(int userId);

        /// <summary>
        /// The get splash screen reports.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="PagedReportsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSplashScreenReports?userId={userId}&pageIndex={pageIndex}&pageSize={pageSize}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        PagedReportsDTO GetSplashScreenReports(int userId, int pageIndex, int pageSize);

        /// <summary>
        /// The get splash screen.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SplashScreenDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSplashScreen?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SplashScreenDTO GetSplashScreen(int userId);

        #endregion
    }
}