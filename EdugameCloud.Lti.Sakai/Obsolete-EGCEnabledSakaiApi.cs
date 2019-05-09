//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Threading.Tasks;
//using Esynctraining.Core.Domain;
//using Esynctraining.Core.Logging;
//using Esynctraining.Core.Providers;
//using Esynctraining.Lti.Lms.Common.API.Sakai;
//using Esynctraining.Lti.Lms.Common.Constants;
//using Esynctraining.Lti.Lms.Common.Dto;

//namespace EdugameCloud.Lti.Sakai
//{
//    public sealed class EGCEnabledSakaiApi : SakaiLmsApi, IEGCEnabledSakaiApi
//    {
//        public EGCEnabledSakaiApi(ApplicationSettingsProvider settings, ILogger logger)
//            : base(settings, logger)
//        { }


//        protected override string SakaiServiceShortName => "edugamecloud";

//        public async Task<OperationResultWithData<IEnumerable<LmsQuizInfoDTO>>> GetItemsInfoForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey)
//        {
//            try
//            {
//                var quizResult = await LoginIfNecessaryAsync(
//                    null,
//                    async c =>
//                    {
//                        var functionName = isSurvey
//                            ? "local_edugamecloud_get_total_survey_list"
//                            : "local_edugamecloud_get_total_quiz_list";

//                        var pairs = new Dictionary<string, string>
//                        {
//                            { "wsfunction", functionName },
//                            { "wstoken", c.Token },
//                            { "course", (string)licenseSettings[LmsUserSettingNames.CourseId]}
//                        };

//                        var xmlDoc = await UploadValuesAsync(c.Url, pairs);

//                        return SakaiQuizInfoParser.Parse(xmlDoc, isSurvey);
//                    },
//                    licenseSettings);

//                if (quizResult.Data == null)
//                {
//                    var error = quizResult.Error ?? "Sakai XML. Unable to retrive result from API";

//                    _logger.ErrorFormat("[EGCEnabledSakaiApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}. Error: {2}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey, error);

//                    return OperationResultWithData<IEnumerable<LmsQuizInfoDTO>>.Error(error);
//                }

//                var result = quizResult.Data;
//                return ((IEnumerable<LmsQuizInfoDTO>) result).ToSuccessResult();
//            }
//            catch (Exception ex)
//            {
//                _logger.ErrorFormat(ex, "[EGCEnabledSakaiApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey);
//                throw;
//            }
//        }

//        /// <summary>
//        /// The get quiz list for user.
//        /// </summary>
//        /// <param name="lmsUserParameters">
//        /// The LMS User Parameters.
//        /// </param>
//        /// <param name="isSurvey">
//        /// The is Survey.
//        /// </param>
//        /// <param name="quizIds">
//        /// The quiz Ids.
//        /// </param>
//        /// <param name="error">
//        /// The error.
//        /// </param>
//        /// <returns>
//        /// The <see cref="IEnumerable{LmsQuizDTO}"/>.
//        /// </returns>
//        public async Task<OperationResultWithData<IEnumerable<LmsQuizDTO>>> GetItemsForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey, IEnumerable<int> quizIds)
//        {
//            try
//            {
//                var result = new List<LmsQuizDTO>();

//                foreach (int quizId in quizIds)
//                {
//                    int id = quizId;
//                    var quizResult = await LoginIfNecessaryAsync(
//                        null,
//                        async c =>
//                        {
//                            var pairs = new Dictionary<string, string>
//                            {
//                                { "wsfunction", isSurvey ? "local_edugamecloud_get_survey_by_id" : "local_edugamecloud_get_quiz_by_id" },
//                                { "wstoken", c.Token },
//                                {  isSurvey ? "surveyId" : "quizId",  id.ToString(CultureInfo.InvariantCulture) }
//                            };

//                            var xmlDoc = await UploadValuesAsync(c.Url, pairs);
//                            string errorMessage = string.Empty;
//                            string err = string.Empty;
//                            return SakaiQuizParser2.Parse(xmlDoc, ref errorMessage, ref err);
//                        },
//                        licenseSettings);

//                    if (quizResult.Data == null)
//                    {
//                        var error = quizResult.Error ?? "Sakai XML. Unable to retrive result from API";
//                        _logger.ErrorFormat("[EGCEnabledSakaiApi.GetItemsForUser] LmsUserParametersId:{0}. IsSurvey:{1}. Error: {2}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey, error);
//                        return OperationResultWithData<IEnumerable<LmsQuizDTO>>.Error(error);
//                    }

//                    result.Add(quizResult.Data);
//                }

//                return ((IEnumerable<LmsQuizDTO>) result).ToSuccessResult();
//            }
//            catch (Exception ex)
//            {
//                _logger.ErrorFormat(ex, "[EGCEnabledSakaiApi.GetItemsInfoForUser] LmsUserParametersId:{0}. IsSurvey:{1}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey);
//                throw;
//            }
//        }

//        /// <summary>
//        /// The send answers.
//        /// </summary>
//        /// <param name="lmsUserParameters">
//        /// The LMS user parameters.
//        /// </param>
//        /// <param name="json">
//        /// The JSON.
//        /// </param>
//        /// <param name="isSurvey">
//        /// The is Survey.
//        /// </param>
//        public async Task SendAnswersAsync(Dictionary<string, object> licenseSettings, string json, bool isSurvey, string[] answers)
//        {
//            string error;

//            try
//            {
//                // ReSharper disable once UnusedVariable
//                var quizResult = await LoginIfNecessaryAsync(
//                    null,
//                    async c =>
//                    {
//                        json = json.Replace("\"", "\"");
//                        var pairs = new Dictionary<string, string>
//                        {
//                            { "wsfunction", isSurvey ? "local_edugamecloud_save_external_survey_report" : "local_edugamecloud_save_external_quiz_report" },
//                            { "wstoken", c.Token },
//                            { "reportObject", json }
//                        };

//                        var xmlDoc = await UploadValuesAsync(c.Url, pairs);

//                        string errorMessage = string.Empty;
//                        string err = string.Empty;
//                        var result = SakaiQuizParser2.Parse(xmlDoc, ref errorMessage, ref err);

//                        if (!string.IsNullOrWhiteSpace(errorMessage) || !string.IsNullOrWhiteSpace(err))
//                        {
//                            _logger.ErrorFormat("[EGCEnabledSakaiApi.SendAnswers.Parsing] LmsUserParametersId:{0}. IsSurvey:{1}. ErrorMessage:{2};{3}. JSON:{4}.",
//                                licenseSettings[LmsUserSettingNames.SessionId],
//                                isSurvey,
//                                errorMessage,
//                                err,
//                                json);
//                        }

//                        return result;
//                    },
//                    licenseSettings);
//            }
//            catch (Exception ex)
//            {
//                _logger.ErrorFormat(ex, "[EGCEnabledSakaiApi.SendAnswers] LmsUserParametersId:{0}. IsSurvey:{1}. JSON:{2}.", licenseSettings[LmsUserSettingNames.SessionId], isSurvey, json);
//                throw;
//            }
//        }

//        public async Task PublishQuiz(Dictionary<string, object> licenseSettings, string courseId, int quizId)
//        {
//            throw new NotImplementedException();
//        }

//    }

//}