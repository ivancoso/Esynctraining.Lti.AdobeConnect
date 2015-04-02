namespace EdugameCloud.Lti.API
{
    using System.Collections.Generic;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using NHibernate.Mapping;

    /// <summary>
    /// The EGC enabled LMS API interface.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public interface IEGCEnabledLmsAPI : ILmsAPI
    {
        /// <summary>
        /// The get items info for user.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The LMS user parameters.
        /// </param>
        /// <param name="isSurvey">
        /// The is survey.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{LmsQuizInfoDTO}"/>.
        /// </returns>
        IEnumerable<LmsQuizInfoDTO> GetItemsInfoForUser(LmsUserParameters lmsUserParameters, bool isSurvey, out string error);

        /// <summary>
        /// The get quiz list for user.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The LMS User Parameters.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <param name="quizIds">
        /// The quiz Ids.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        IEnumerable<LmsQuizDTO> GetItemsForUser(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds, out string error);

        /// <summary>
        /// The send answers.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The LMS user parameters.
        /// </param>
        /// <param name="json">
        /// The JSON.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <param name="answers">
        /// The answers.
        /// </param>
        void SendAnswers(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers = null);
    }
}
