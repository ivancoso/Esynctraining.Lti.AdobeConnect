﻿namespace EdugameCloud.Lti.Moodle
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using Esynctraining.Core.Logging;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
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

        public IEnumerable<LmsQuizInfoDTO> GetItemsInfoForUser(LmsUserParameters lmsUserParameters, bool isSurvey, out string error)
        {
            try
            {
                var quizResult = this.LoginIfNecessary(
                    null,
                    c =>
                    {
                        var functionName = isSurvey
                            ? "local_edugamecloud_get_total_survey_list"
                            : "local_edugamecloud_get_total_quiz_list";

                        var pairs = new NameValueCollection
                        {
                            { "wsfunction", functionName },
                            { "wstoken", c.Token },
                            { "course", lmsUserParameters.Course.ToString( CultureInfo.InvariantCulture) }
                        };

                        var xmlDoc = UploadValues(c.Url, pairs);

                        return MoodleQuizInfoParser.Parse(xmlDoc, isSurvey);
                    },
                    out error,
                    lmsUserParameters.LmsUser);

                if (quizResult == null)
                {
                    error = error ?? "Moodle XML. Unable to retrive result from API";

                    logger.ErrorFormat("[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}. Error: {2}.", lmsUserParameters.Id, isSurvey, error);

                    return Enumerable.Empty<LmsQuizInfoDTO>();
                }

                error = string.Empty;
                return quizResult;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}.", lmsUserParameters.Id, isSurvey);
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
        public IEnumerable<LmsQuizDTO> GetItemsForUser(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds, out string error)
        {
            try
            {
                var result = new List<LmsQuizDTO>();

                foreach (int quizId in quizIds)
                {
                    int id = quizId;
                    var quizResult = this.LoginIfNecessary(
                        null,
                        c =>
                        {
                            var pairs = new NameValueCollection
                            {
                                { "wsfunction", isSurvey ? "local_edugamecloud_get_survey_by_id" : "local_edugamecloud_get_quiz_by_id" },
                                { "wstoken", c.Token },
                                {  isSurvey ? "surveyId" : "quizId",  id.ToString(CultureInfo.InvariantCulture) }
                            };

                            var xmlDoc = UploadValues(c.Url, pairs);
                            string errorMessage = string.Empty;
                            string err = string.Empty;
                            return MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);
                        },
                        out error,
                        lmsUserParameters.LmsUser);
                    if (quizResult == null)
                    {
                        error = error ?? "Moodle XML. Unable to retrive result from API";

                        logger.ErrorFormat("[EGCEnabledMoodleApi.GetItemsForUser] LmsUserParametersId:{0}. IsSurvey:{1}. Error: {2}.", lmsUserParameters.Id, isSurvey, error);

                        return result;
                    }

                    result.Add(quizResult);
                }

                error = string.Empty;
                return result;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}.", lmsUserParameters.Id, isSurvey);
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
        public void SendAnswers(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers)
        {
            string error;

            try
            {
                // ReSharper disable once UnusedVariable
                var quizResult = this.LoginIfNecessary(
                    null,
                    c =>
                    {
                        json = json.Replace("\"", "\"");
                        var pairs = new NameValueCollection
                        {
                            { "wsfunction", isSurvey ? "local_edugamecloud_save_external_survey_report" : "local_edugamecloud_save_external_quiz_report" },
                            { "wstoken", c.Token },
                            { "reportObject", json }
                        };

                        var xmlDoc = UploadValues(c.Url, pairs);

                        string errorMessage = string.Empty;
                        string err = string.Empty;
                        var result = MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);

                        if (!string.IsNullOrWhiteSpace(errorMessage) || !string.IsNullOrWhiteSpace(err))
                        {
                            logger.ErrorFormat("[EGCEnabledMoodleApi.SendAnswers.Parsing] LmsUserParametersId:{0}. IsSurvey:{1}. ErrorMessage:{2};{3}. JSON:{4}.",
                                lmsUserParameters.Id, 
                                isSurvey,
                                errorMessage,
                                err,
                                json);
                        }

                        return result;
                    },
                    out error,
                    lmsUserParameters.LmsUser);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.SendAnswers] LmsUserParametersId:{0}. IsSurvey:{1}. JSON:{2}.", lmsUserParameters.Id, isSurvey, json);
                throw;
            }
        }

    }

}
