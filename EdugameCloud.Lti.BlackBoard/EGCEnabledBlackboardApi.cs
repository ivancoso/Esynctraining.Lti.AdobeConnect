using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.BlackBoardClient;
using Esynctraining.BlackBoardClient.Content;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.BlackBoard
{
    public sealed class EGCEnabledBlackboardApi : SoapBlackBoardApi, IEGCEnabledBlackBoardApi
    {
        public EGCEnabledBlackboardApi(ILogger logger)
            : base(logger)
        {
        }

        public async Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey)
        {
            var quizzes = await GetItemsForUserAsync(licenseSettings, isSurvey, null);
            if (quizzes.Data == null)
                throw new InvalidOperationException("There was a problem establishing connection to an api");

            var result = quizzes.Data.Select(q => new LmsQuizInfoDTO
            {
                id = q.id,
                name = q.title,
                course = q.course,
                courseName = q.courseName,
                lastModifiedLMS = q.lastModifiedLMS,
                isPublished = q.published
            });

            return (Data: result, Error: quizzes.Error);
        }

        public async Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey, IEnumerable<int> quizIds)
        {
            WebserviceWrapper client = null;
            string error;
            var tests = this.LoginIfNecessary(
                ref client,
                c =>
                {
                    var egc = c.getEdugameCloudWrapper();
                    var content = c.getContentWrapper();

                    var tos = content.getTOCsByCourseId((string)licenseSettings[LmsUserSettingNames.CourseId]);

                    if (tos != null)
                    {
                        tos =
                            tos.Where(
                                to =>
                                to.label != null
                                && to.label.Equals("content", StringComparison.InvariantCultureIgnoreCase))
                                .ToArray();
                    }

                    ContentVO[] tsts = null;

                    if (tos != null && tos.Any())
                    {
                        var contentFilter = new ContentFilter()
                        {
                            filterType = 3,
                            filterTypeSpecified = true,
                            contentId = tos.First().courseContentsId
                        };

                        var loaded = content.loadFilteredContent((string)licenseSettings[LmsUserSettingNames.CourseId], contentFilter);
                        if (loaded != null)
                        {
                            tsts =
                                loaded.Where(
                                    l =>
                                    l.contentHandler != null
                                    && l.contentHandler.Equals(isSurvey ? "resource/x-bb-asmt-survey-link" : "resource/x-bb-asmt-test-link")).ToArray();
                        }
                    }


                    string testData = string.Empty;

                    if (tsts != null)
                    {
                        var quizDTO = new List<LmsQuizDTO>();

                        foreach (var t in tsts)
                        {
                            var lqd = new LmsQuizDTO()
                            {
                                course = (string)licenseSettings[LmsUserSettingNames.CourseId],
                                courseName = (string)licenseSettings[LmsUserSettingNames.CourseId],
                                description = t.body.ClearName(),
                                title = t.title.ClearName(),
                                id = BlackboardHelper.GetBBId(t.id),
                                published = true
                            };
                            if (quizIds != null && !quizIds.Contains(lqd.id))
                            {
                                continue;
                            }
                            testData = egc.getAssessmentDetails(t.id, isSurvey);

                            if (testData != null && !testData.StartsWith("Error", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var td = JsonConvert.DeserializeObject<BBAssessmentDTO>(testData);
                                lqd.question_list = BlackboardQuizParser.ParseQuestions(td, testData);
                                var repoImages = td.images as JToken;
                                if (repoImages != null)
                                {
                                    var temp = repoImages.ToObject<Dictionary<string, string>>();
                                    lqd.Images = temp.ToDictionary(x => x.Key, x => x.Value);
                                } 
                            }
                            quizDTO.Add(lqd);
                        }

                        return quizDTO.ToList();
                    }

                    return new List<LmsQuizDTO> { };
                },
                licenseSettings,
                out error);

            return (Data: tests, Error: error);
        }

        public async Task SendAnswersAsync(Dictionary<string, object> licenseSettings, string contentId, bool isSurvey, string[] answers)
        {
            WebserviceWrapper client = null;

            string contentId1 = contentId;
            var tests = this.LoginIfNecessary(
                ref client,
                c =>
                    {
                        var egc = c.getEdugameCloudWrapper();

                        return egc.submitTestResult(
                            contentId1,
                            (string)licenseSettings[LmsUserSettingNames.Token],
                            answers,
                            null,
                            isSurvey);
                    },
                licenseSettings,
                out contentId);
        }

        public async Task PublishQuiz(Dictionary<string, object> licenseSettings, string courseId, int quizId)
        {
            throw new NotImplementedException();
        }
    }
}
