using EdugameCloud.Lti.Core.Constants;

namespace EdugameCloud.Lti.Moodle
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;

    /// <summary>
    /// The Moodle API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class EGCEnabledMoodleApi : MoodleApi, IEGCEnabledMoodleApi
    {
        public EGCEnabledMoodleApi(ApplicationSettingsProvider settings, ILogger logger)
            : base(settings, logger)
        { }
        

        protected override string MoodleServiceShortName
        {
            get { return "edugamecloud"; }
        }

        public async Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey)
        {
            string error = null;
            try
            {
                var moodleServiceToken = lmsUserParameters.CompanyLms.GetSetting<string>(LmsCompanySettingNames.MoodleQuizServiceToken);
                var quizResult = !string.IsNullOrEmpty(moodleServiceToken)
                    ? GetQuizzes(moodleServiceToken, isSurvey, lmsUserParameters.Course, lmsUserParameters.CompanyLms)
                    : LoginIfNecessary(
                    null,
                    c =>
                    {
                        return GetQuizzes(c.Token, isSurvey, lmsUserParameters.Course, lmsUserParameters.CompanyLms);
                    },
                    out error,
                    lmsUserParameters.LmsUser);

                if (quizResult == null)
                {
                    error = error ?? "Moodle XML. Unable to retrive result from API";

                    _logger.ErrorFormat("[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}. Error: {2}.", lmsUserParameters.Id, isSurvey, error);

                    return (Data: Enumerable.Empty<LmsQuizInfoDTO>(), Error: error);
                }

                return (Data: quizResult, Error: string.Empty);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}.", lmsUserParameters.Id, isSurvey);
                throw;
            }
        }

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
        /// The <see cref="IEnumerable{LmsQuizDTO}"/>.
        /// </returns>
        public async Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds)
        {
            try
            {
                var result = new List<LmsQuizDTO>();
                string error = null;
                foreach (int quizId in quizIds)
                {
                    int id = quizId;
                    var moodleServiceToken = lmsUserParameters.CompanyLms.GetSetting<string>(LmsCompanySettingNames.MoodleQuizServiceToken);
                    var quizResult = !string.IsNullOrEmpty(moodleServiceToken)
                        ? GetQuiz(moodleServiceToken, isSurvey, quizId, lmsUserParameters.CompanyLms)
                        : LoginIfNecessary(
                        null,
                        c =>
                        {
                            return GetQuiz(c.Token, isSurvey, quizId, lmsUserParameters.CompanyLms);
                        },
                        out error,
                        lmsUserParameters.LmsUser);

                    if (quizResult == null)
                    {
                        error = error ?? "Moodle XML. Unable to retrive result from API";

                        _logger.ErrorFormat("[EGCEnabledMoodleApi.GetItemsForUser] LmsUserParametersId:{0}. IsSurvey:{1}. Error: {2}.", lmsUserParameters.Id, isSurvey, error);

                        return (Data: result, Error: error);
                    }

                    result.Add(quizResult);
                }

                return (Data: result, Error: string.Empty);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}.", lmsUserParameters.Id, isSurvey);
                throw;
            }
        }

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
        public async Task SendAnswersAsync(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers)
        {
            string error;

            try
            {
                var moodleServiceToken = lmsUserParameters.CompanyLms.GetSetting<string>(LmsCompanySettingNames.MoodleQuizServiceToken);
                var quizResult = !string.IsNullOrEmpty(moodleServiceToken)
                    ? SendQuiz(moodleServiceToken, isSurvey, json, lmsUserParameters)
                    : LoginIfNecessary(
                    null,
                    c =>
                    {
                        return SendQuiz(c.Token, isSurvey, json, lmsUserParameters);
                    },
                    out error,
                    lmsUserParameters.LmsUser);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.SendAnswers] LmsUserParametersId:{0}. IsSurvey:{1}. JSON:{2}.", lmsUserParameters.Id, isSurvey, json);
                throw;
            }
        }

        public void PublishQuiz(LmsUserParameters lmsUserParameters, int courseId, int quizId)
        {
            throw new NotImplementedException();
        }


        private LmsQuizDTO SendQuiz(string token, bool isSurvey, string json, LmsUserParameters lmsUserParameters)
        {
            json = json.Replace("\"", "\"");
            var pairs = new Dictionary<string, string>
            {
                { "wsfunction", isSurvey ? "local_edugamecloud_save_external_survey_report" : "local_edugamecloud_save_external_quiz_report" },
                { "wstoken",  token },
                { "reportObject", json }
            };
            var url = GetServicesUrl(lmsUserParameters.CompanyLms);
            var xmlDoc = UploadValues(url, pairs);

            string errorMessage = string.Empty;
            string err = string.Empty;
            var result = MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);

            if (!string.IsNullOrWhiteSpace(errorMessage) || !string.IsNullOrWhiteSpace(err))
            {
                _logger.ErrorFormat("[EGCEnabledMoodleApi.SendAnswers.Parsing] LmsUserParametersId:{0}. IsSurvey:{1}. ErrorMessage:{2};{3}. JSON:{4}.",
                    lmsUserParameters.Id,
                    isSurvey,
                    errorMessage,
                    err,
                    json);
            }

            return result;
        }

        private IEnumerable<LmsQuizInfoDTO> GetQuizzes(string token, bool isSurvey, int courseId, ILmsLicense lmsCompany)
        {
            var functionName = isSurvey
                ? "local_edugamecloud_get_total_survey_list"
                : "local_edugamecloud_get_total_quiz_list";

            var pairs = new Dictionary<string, string>
            {
                { "wsfunction", functionName },
                { "wstoken",  token },
                { "course", courseId.ToString( CultureInfo.InvariantCulture) }
            };
            var url = GetServicesUrl(lmsCompany);
            var xmlDoc = UploadValues(url, pairs);

            return MoodleQuizInfoParser.Parse(xmlDoc, isSurvey);
        }

        private LmsQuizDTO GetQuiz(string token, bool isSurvey, int courseId, ILmsLicense lmsCompany)
        {
            var pairs = new Dictionary<string, string>
            {
                { "wsfunction", isSurvey ? "local_edugamecloud_get_survey_by_id" : "local_edugamecloud_get_quiz_by_id" },
                { "wstoken",  token },
                {  isSurvey ? "surveyId" : "quizId",  courseId.ToString(CultureInfo.InvariantCulture) }
            };

            var url = GetServicesUrl(lmsCompany);
            var xmlDoc = UploadValues(url, pairs);
            string errorMessage = string.Empty;
            string err = string.Empty;
            return MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);
        }

    }

}
