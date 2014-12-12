namespace EdugameCloud.Lti.API.Moodle
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using Castle.Core.Logging;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.EntityParsing;

    using Esynctraining.Core.Providers;

    /// <summary>
    /// The Moodle API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class EGCEnabledMoodleAPI : MoodleAPI, IEGCEnabledLmsAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EGCEnabledMoodleAPI"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public EGCEnabledMoodleAPI(ApplicationSettingsProvider settings, ILogger logger) : base(settings, logger)
        {
        }

        /// <summary>
        /// The Moodle service short name.
        /// </summary>
        protected override string MoodleServiceShortName
        {
            get
            {
                return "edugamecloud";
            }
        }

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
        public IEnumerable<LmsQuizInfoDTO> GetItemsInfoForUser(LmsUserParameters lmsUserParameters, bool isSurvey, out string error)
        {
            var quizResult = this.LoginIfNecessary(
                null,
                c => {

                        var functionName = isSurvey
                                               ? "local_edugamecloud_get_total_survey_list"
                                               : "local_edugamecloud_get_total_quiz_list";
                        var pairs = new NameValueCollection
                                        {
                                            { "wsfunction", functionName },
                                            { "wstoken", c.Token },
                                            {
                                                "course",
                                                lmsUserParameters.Course.ToString(
                                                    CultureInfo.InvariantCulture)
                                            }
                                        };

                        var xmlDoc = this.UploadValues(c.Url, pairs);

                        return MoodleQuizInfoParser.Parse(xmlDoc, isSurvey);
                    },
                out error,
                lmsUserParameters.LmsUser);

            if (quizResult == null)
            {
                error = error ?? "Moodle XML. Unable to retrive result from API";
                return new List<LmsQuizInfoDTO>();
            }

            error = string.Empty;
            return quizResult;
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
        public IEnumerable<LmsQuizDTO> GetItemsForUser(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds, out string error)
        {
            var result = new List<LmsQuizDTO>();
            
            foreach (var quizId in quizIds)
            {
                int id = quizId;
                var quizResult = this.LoginIfNecessary(
                    null,
                    c =>
                        {
                            var pairs = new NameValueCollection
                                            {
                                                {
                                                    "wsfunction",
                                                    isSurvey ? "local_edugamecloud_get_survey_by_id" : "local_edugamecloud_get_quiz_by_id"
                                                },
                                                { "wstoken", c.Token },
                                                { 
                                                    isSurvey ? "surveyId" : "quizId", 
                                                    id.ToString(CultureInfo.InvariantCulture) 
                                                }
                                            };

                            var xmlDoc = this.UploadValues(c.Url, pairs);

                            string errorMessage = string.Empty, err = string.Empty;
                            return MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);
                        },
                    out error,
                    lmsUserParameters.LmsUser);
                if (quizResult == null)
                {
                    error = error ?? "Moodle XML. Unable to retrive result from API";
                    return result;
                }

                result.Add(quizResult);
            }
            
            error = string.Empty;
            return result;
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
        public void SendAnswers(LmsUserParameters lmsUserParameters, string json, bool isSurvey)
        {
            string error;

            // ReSharper disable once UnusedVariable
            var quizResult = this.LoginIfNecessary(
                null,
                c =>
                    {
                        json = json.Replace("\"", "\"");
                        var pairs = new NameValueCollection
                                        {
                                            {
                                                "wsfunction",
                                                isSurvey ? "local_edugamecloud_save_external_survey_report" : "local_edugamecloud_save_external_quiz_report"
                                            },
                                            { "wstoken", c.Token },
                                            { "reportObject", json }
                                        };

                        var xmlDoc = this.UploadValues(c.Url, pairs);

                        string errorMessage = string.Empty, err = string.Empty;
                        return MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);
                    },
                out error,
                lmsUserParameters.LmsUser);
        }
    }
}
