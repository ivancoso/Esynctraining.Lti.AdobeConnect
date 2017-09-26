namespace EdugameCloud.Lti.API
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;

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
        Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey);

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
        Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds);

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
        Task SendAnswersAsync(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers = null);

        void PublishQuiz(LmsUserParameters lmsUserParameters, int courseId, int quizId);
    }
}
