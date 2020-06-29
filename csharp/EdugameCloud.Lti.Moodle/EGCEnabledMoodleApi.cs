using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Moodle;
using Esynctraining.Core.Json;
using System.Net.Http;

namespace EdugameCloud.Lti.Moodle
{
    public sealed class EGCEnabledMoodleApi : MoodleApi, IEGCEnabledMoodleApi
    {
        public EGCEnabledMoodleApi(IHttpClientFactory httpClientFactory, ILogger logger, IJsonDeserializer jsonDeserializer)
            : base(jsonDeserializer, logger, httpClientFactory)
        { }

        protected override string MoodleServiceShortName
        {
            get { return "edugamecloud"; }
        }

        public async Task<OperationResultWithData<IEnumerable<LmsQuizInfoDTO>>> GetItemsInfoForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey)
        {
            string error = null;
            try
            {
                var moodleServiceToken = (string)licenseSettings[LmsLicenseSettingNames.MoodleQuizServiceToken];
                //var quizResult = !string.IsNullOrEmpty(moodleServiceToken)
                //    ? GetQuizzes(moodleServiceToken, isSurvey, lmsUserParameters.Course, lmsUserParameters.CompanyLms)
                //    : LoginIfNecessary(
                //    null,
                //    c =>
                //    {
                //        return GetQuizzes(c.Token, isSurvey, lmsUserParameters.Course, lmsUserParameters.CompanyLms);
                //    },
                //    out error,
                //    lmsUserParameters.LmsUser);
                //-----------------
                IEnumerable<LmsQuizInfoDTO> quizResult;
                if (!string.IsNullOrEmpty(moodleServiceToken))
                {
                    quizResult = await GetQuizzes(moodleServiceToken, isSurvey, (string)licenseSettings[LmsUserSettingNames.CourseId], licenseSettings);
                }
                else
                {
                    var quizResultTuple = await LoginIfNecessary(
                    null,
                    async c =>
                    {
                        return await GetQuizzes(c.Token, isSurvey, (string)licenseSettings[LmsUserSettingNames.CourseId], licenseSettings);
                    },
                    licenseSettings);

                    //popov
                    quizResult = await quizResultTuple.result;
                    error = quizResultTuple.error;
                }
                //-----------------

                if (quizResult == null)
                {
                    error = error ?? "Moodle XML. Unable to retrive result from API";
                    Logger.ErrorFormat("[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}. Error: {2}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey, error);
                    return OperationResultWithData<IEnumerable<LmsQuizInfoDTO>>.Error(error);
                }

                return quizResult.ToSuccessResult();
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey);
                throw;
            }
        }

        /// <summary>
        /// The get quiz list for user.
        /// </summary>
        public async Task<OperationResultWithData<IEnumerable<LmsQuizDTO>>> GetItemsForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey, IEnumerable<int> quizIds)
        {
            try
            {
                var result = new List<LmsQuizDTO>();
                string error = null;
                foreach (int quizId in quizIds)
                {
                    int id = quizId;
                    var moodleServiceToken = (string)licenseSettings[LmsLicenseSettingNames.MoodleQuizServiceToken];
                    //var quizResult = !string.IsNullOrEmpty(moodleServiceToken)
                    //    ? GetQuiz(moodleServiceToken, isSurvey, quizId, lmsUserParameters.CompanyLms)
                    //    : LoginIfNecessary(
                    //    null,
                    //    c =>
                    //    {
                    //        return GetQuiz(c.Token, isSurvey, quizId, lmsUserParameters.CompanyLms);
                    //    },
                    //    out error,
                    //    lmsUserParameters.LmsUser);

                    //-------------------------------------------------
                    LmsQuizDTO quizResult;
                    if (!string.IsNullOrEmpty(moodleServiceToken))
                    {
                        quizResult = await GetQuiz(moodleServiceToken, isSurvey, quizId, licenseSettings);
                    }
                    else
                    {
                        var quizResultTuple = await LoginIfNecessary(
                            null,
                            async c =>
                            {
                                return await GetQuiz(c.Token, isSurvey, quizId, licenseSettings);
                            },
                            licenseSettings);
                        //popov
                        quizResult = await quizResultTuple.result;
                        error = quizResultTuple.error;
                    }
                    //-------------------------------------------------

                    if (quizResult == null)
                    {
                        error = error ?? "Moodle XML. Unable to retrive result from API";
                        Logger.ErrorFormat("[EGCEnabledMoodleApi.GetItemsForUser] LmsUserParametersId:{0}. IsSurvey:{1}. Error: {2}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey, error);
                        return OperationResultWithData<IEnumerable<LmsQuizDTO>>.Error(error);
                    }

                    result.Add(quizResult);
                }

                return ((IEnumerable<LmsQuizDTO>) result).ToSuccessResult();
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey);
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
        public async Task SendAnswersAsync(Dictionary<string, object> licenseSettings, string json, bool isSurvey, string[] answers)
        {
            string error;

            try
            {
                var moodleServiceToken = (string)licenseSettings[LmsLicenseSettingNames.MoodleQuizServiceToken];
                //var quizResult = !string.IsNullOrEmpty(moodleServiceToken)
                //    ? SendQuiz(moodleServiceToken, isSurvey, json, lmsUserParameters)
                //    : LoginIfNecessary(
                //    null,
                //    c =>
                //    {
                //        return SendQuiz(c.Token, isSurvey, json, lmsUserParameters);
                //    },
                //    out error,
                //    lmsUserParameters.LmsUser);
                //-----------------------------------------------
                LmsQuizDTO quizResult;
                if (!string.IsNullOrEmpty(moodleServiceToken))
                {
                    quizResult = await SendQuiz(moodleServiceToken, isSurvey, json, licenseSettings);
                }
                else
                {
                    var quizResultTuple = await LoginIfNecessary(
                        null,
                        c =>
                        {
                            return SendQuiz(c.Token, isSurvey, json, licenseSettings);
                        },
                        licenseSettings);

                    //popov
                    quizResult = await quizResultTuple.result;
                    error = quizResultTuple.error;

                }
                //-----------------------------------------------
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "[EGCEnabledMoodleApi.SendAnswers] LmsUserParametersId:{0}. IsSurvey:{1}. JSON:{2}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey, json);
                throw;
            }
        }

        public async Task PublishQuiz(Dictionary<string, object> licenseSettings, string courseId, int quizId)
        {
            throw new NotImplementedException();
        }


        private async Task<LmsQuizDTO> SendQuiz(string token, bool isSurvey, string json, Dictionary<string, object> licenseSettings)
        {
            json = json.Replace("\"", "\"");
            var pairs = new Dictionary<string, string>
            {
                { "wsfunction", isSurvey ? "local_edugamecloud_save_external_survey_report" : "local_edugamecloud_save_external_quiz_report" },
                { "wstoken",  token },
                { "reportObject", json }
            };
            var url = GetServicesUrl(licenseSettings);
            var xmlDoc = await UploadValues(url, pairs);

            string errorMessage = string.Empty;
            string err = string.Empty;
            var result = MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);

            if (!string.IsNullOrWhiteSpace(errorMessage) || !string.IsNullOrWhiteSpace(err))
            {
                Logger.ErrorFormat("[EGCEnabledMoodleApi.SendAnswers.Parsing] LmsUserParametersId:{0}. IsSurvey:{1}. ErrorMessage:{2};{3}. JSON:{4}.",
                    licenseSettings[LmsUserSettingNames.SessionId],
                    isSurvey,
                    errorMessage,
                    err,
                    json);
            }

            return result;
        }

        private async Task<IEnumerable<LmsQuizInfoDTO>> GetQuizzes(string token, bool isSurvey, string courseId, Dictionary<string, object> licenseSettings)
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
            var url = GetServicesUrl(licenseSettings);
            var xmlDoc = await UploadValues(url, pairs);

            return MoodleQuizInfoParser.Parse(xmlDoc, isSurvey);
        }

        private async Task<LmsQuizDTO> GetQuiz(string token, bool isSurvey, int courseId, Dictionary<string, object> licenseSettings)
        {
            var pairs = new Dictionary<string, string>
            {
                { "wsfunction", isSurvey ? "local_edugamecloud_get_survey_by_id" : "local_edugamecloud_get_quiz_by_id" },
                { "wstoken",  token },
                {  isSurvey ? "surveyId" : "quizId",  courseId.ToString(CultureInfo.InvariantCulture) }
            };

            var url = GetServicesUrl(licenseSettings);
            var xmlDoc = await UploadValues(url, pairs);
            string errorMessage = string.Empty;
            string err = string.Empty;
            return MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);
        }

    }

}
