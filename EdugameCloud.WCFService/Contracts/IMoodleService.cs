namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The MoodleService interface.
    /// </summary>
    [ServiceContract]
    public interface IMoodleService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get quiz.
        /// </summary>
        /// <param name="userInfo">
        /// The user Info.
        /// </param>
        /// <returns>
        /// The <see cref="MoodleQuizInfoDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        MoodleQuizInfoDTO GetQuizzesForUser(MoodleUserInfoDTO userInfo);

        /// <summary>
        /// The get surveys for user
        /// </summary>
        /// <param name="userInfo">
        /// The user info
        /// </param>
        /// <returns>
        /// The <see cref="MoodleQuizInfoDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        MoodleQuizInfoDTO GetSurveysForUser(MoodleUserInfoDTO userInfo);

        /// <summary>
        /// The convert quiz.
        /// </summary>
        /// <param name="quiz">
        /// The quiz
        /// </param>
        /// <returns>
        /// The <see cref="QuizesAndSubModuleItemsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuizesAndSubModuleItemsDTO ConvertQuizzes(LmsQuizConvertDTO quiz);

        /// <summary>
        /// The convert surveys.
        /// </summary>
        /// <param name="survey">
        /// The survey
        /// </param>
        /// <returns>
        /// The <see cref="SurveysAndSubModuleItemsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveysAndSubModuleItemsDTO ConvertSurveys(LmsQuizConvertDTO survey);

        /// <summary>
        /// The get authentication parameters by id
        /// </summary>
        /// <param name="id">
        /// The id
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserParametersDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        LmsUserParametersDTO GetAuthenticationParametersById(string id);

        /// <summary>
        /// The test.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void Test();

        #endregion
    }
}
