using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Sakai.Dto;
using Esynctraining.HttpClient;
using Esynctraining.Lti.Lms.Common.API.Sakai;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiApi : ISakaiApi
    {
        private static readonly HttpClientWrapper _httpClientWrapper = new HttpClientWrapper();

        public SakaiApi()
        {
        }

        public async Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey)
        {
            var quizDtos = await GetItemsForUserAsync(licenseSettings, isSurvey, null);

            var result = quizDtos.Data.Select(q => new LmsQuizInfoDTO
            {
                id = q.id,
                name = q.title,
                course = q.course,
                courseName = q.courseName,
                lastModifiedLMS = q.lastModifiedLMS,
                isPublished = q.published
            });

            return (Data: result, Error: quizDtos.Error);
        }

        public async Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey, IEnumerable<int> quizIds)
        {
            var apiParam = new SakaiExtendedParams
            {
                LtiMessageType = "egc_get_assessment_data",
            };

            var courseName = (string)licenseSettings[LmsUserSettingNames.CourseName];
            var testsUrl = $"http://sakai11.esynctraining.com/egcint/service/?lti_message_type=egc_get_assessments&sourcedid={ courseName }&lti_version=LTI-1p0&oauth_consumer_key=esynctraining.com&secret=07951-BAUER-41481-CRLSHM&user_id={licenseSettings[LmsUserSettingNames.UserId]}&ext_sakai_provider_eid={ WebUtility.UrlEncode((string)licenseSettings[LmsUserSettingNames.Username]) }";

            string testsJson = await _httpClientWrapper.DownloadStringAsync(testsUrl);

            SakaiSiteDto[] tests = null;
            if (!testsJson.StartsWith("Error", StringComparison.InvariantCultureIgnoreCase))
            {
                tests = JsonConvert.DeserializeObject<SakaiSiteDto[]>(testsJson);
            }

            if (tests == null)
            {
                throw new InvalidOperationException("Sakai api getting tests method failed!");
            }

            var quizDto = new List<LmsQuizDTO>();
            if (quizIds != null)
                tests = tests.Where(x => quizIds.Contains(x.publishedAssessmentId)).ToArray();
            foreach (var test in tests)
            {
                //var json = JsonConvert.SerializeObject(apiParam);
                var assessmentId = test.publishedAssessmentId;
                var quizzesIds = new List<int>();
                quizzesIds.Add(assessmentId);
                var url = 
                    $"http://sakai11.esynctraining.com/egcint/service/?lti_message_type=egc_get_assessment_data&sourcedid={ courseName }&assessmentId={ assessmentId }&lti_version=LTI-1p0&oauth_consumer_key=esynctraining.com&secret=07951-BAUER-41481-CRLSHM&user_id={licenseSettings[LmsUserSettingNames.UserId]}&ext_sakai_provider_eid={ WebUtility.UrlEncode((string)licenseSettings[LmsUserSettingNames.Username]) }";

                string resp = await _httpClientWrapper.DownloadStringAsync(url);

                var lqd = new LmsQuizDTO()
                {
                    course = (string)licenseSettings[LmsUserSettingNames.CourseId],
                    courseName = courseName,
                    //description = test.name,
                    title = test.name,
                    id = test.publishedAssessmentId,
                    published = true
                };
                //if (quizIds != null && !quizIds.Contains(lqd.id))
                //{
                //    continue;
                //}
                //testData = egc.getAssessmentDetails(t.id, isSurvey);

                if (!resp.StartsWith("Error", StringComparison.InvariantCultureIgnoreCase))
                {
                    var td = JsonConvert.DeserializeObject<BBAssessmentDTO>(resp);
                    lqd.question_list = SakaiQuizParser.ParseQuestions(td, resp);
                    var repoImages = td.images as JToken;
                    if (repoImages != null)
                    {
                        var temp = repoImages.ToObject<Dictionary<string, string>>();
                        lqd.Images = temp.ToDictionary(x => x.Key, x => x.Value);
                    }
                }

                quizDto.Add(lqd);
            }

            return (Data: quizDto.ToList(), Error: string.Empty);
        }

        public async Task SendAnswersAsync(Dictionary<string, object> licenseSettings, string json, bool isSurvey, string[] answers = null)
        {
            var url = 
                $@"http://sakai11.esynctraining.com/egcint/service/?" +
                $"lti_message_type=egc_submit_results2" +
                $"&contentId={ json }" +
                $"&sourcedid={licenseSettings[LmsUserSettingNames.CourseName]}" +
                $"&lti_version=LTI-1p0" +
                $"&oauth_consumer_key=esynctraining.com" +
                $"&secret=07951-BAUER-41481-CRLSHM" +
                $"&ext_sakai_provider_eid={licenseSettings[LmsUserSettingNames.Username]}" +
                $"&user_id={ WebUtility.UrlEncode((string)licenseSettings[LmsUserSettingNames.UserId]) }";

            //stud = @"[\"false\", \"test\", \"2\"]"

            var resultsJson = JsonConvert.SerializeObject(answers);

            await _httpClientWrapper.UploadJsonStringAsync(url, resultsJson);

        }

        public async Task PublishQuiz(Dictionary<string, object> licenseSettings, string courseId, int quizId)
        {
            throw new NotImplementedException();
        }
    }
}