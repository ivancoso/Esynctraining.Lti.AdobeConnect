using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Sakai.Dto;
using Esynctraining.Core.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiApi : ISakaiApi
    {
        private readonly ILogger _logger;

        public SakaiApi(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<LmsQuizInfoDTO> GetItemsInfoForUser(LmsUserParameters lmsUserParameters, bool isSurvey, out string error)
        {
            var quizDtos = GetItemsForUser(lmsUserParameters, isSurvey, null, out error);

            var result = quizDtos.Select(q => new LmsQuizInfoDTO
            {
                id = q.id,
                name = q.title,
                course = q.course,
                courseName = q.courseName,
                lastModifiedLMS = q.lastModifiedLMS,
                isPublished = q.published
            });

            return result;
        }

        public IEnumerable<LmsQuizDTO> GetItemsForUser(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds, out string error)
        {
            var apiParam = new SakaiExtendedParams
            {
                LtiMessageType = "egc_get_assessment_data",
            };

            var course = lmsUserParameters.CourseName;
            var testsUrl = $"http://sakai11.esynctraining.com/egcint/service/?lti_message_type=egc_get_assessments&sourcedid={ course }&lti_version=LTI-1p0&oauth_consumer_key=esynctraining.com&secret=07951-BAUER-41481-CRLSHM&user_id={lmsUserParameters.LmsUser.UserId }&ext_sakai_provider_eid={ HttpUtility.UrlEncode(lmsUserParameters.LmsUser.Username) }";

            string testsJson;

            using (var webClient = new WebClient())
            {
                testsJson = webClient.DownloadString(testsUrl);
            }

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
                string resp;
                var assessmentId = test.publishedAssessmentId;
                var quizzesIds = new List<int>();
                quizzesIds.Add(assessmentId);
                var url = 
                    $"http://sakai11.esynctraining.com/egcint/service/?lti_message_type=egc_get_assessment_data&sourcedid={ course }&assessmentId={ assessmentId }&lti_version=LTI-1p0&oauth_consumer_key=esynctraining.com&secret=07951-BAUER-41481-CRLSHM&user_id={lmsUserParameters.LmsUser.UserId }&ext_sakai_provider_eid={ HttpUtility.UrlEncode(lmsUserParameters.LmsUser.Username) }";
                using (var webClient = new WebClient())
                {
                    resp = webClient.DownloadString(url);
                }

                var lqd = new LmsQuizDTO()
                {
                    course = lmsUserParameters.Course,
                    courseName = lmsUserParameters.CourseName,
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
            error = string.Empty;
            return quizDto.ToList();

        }

        public void SendAnswers(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers = null)
        {
            //var session = GetReadOnlySession(lmsProviderName);
            //LtiParamDTO param = session.LtiSession.LtiParam;
            //var param = SessionModel.
            //var url =GetApiUrl(param);
            var url = 
                $@"http://sakai11.esynctraining.com/egcint/service/?lti_message_type=egc_submit_results2" +
                $"&contentId={json }&sourcedid={ lmsUserParameters.CourseName }&lti_version=LTI-1p0&oauth_consumer_key=esynctraining.com" +
                $"&secret=07951-BAUER-41481-CRLSHM&ext_sakai_provider_eid={ lmsUserParameters.LmsUser.Username }&user_id={ HttpUtility.UrlEncode(lmsUserParameters.LmsUser.UserId) }";

            //stud = @"[\"false\", \"test\", \"2\"]"

            var resultsJson = JsonConvert.SerializeObject(answers);

            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                webClient.UploadString(url, resultsJson);
            }


        }
    }
}