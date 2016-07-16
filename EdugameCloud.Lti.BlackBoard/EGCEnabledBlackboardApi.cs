using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BbWsClient;
using BbWsClient.Content;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.Core.Providers;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.BlackBoard
{
    public sealed class EGCEnabledBlackboardApi : SoapBlackBoardApi, IEGCEnabledBlackBoardApi
    {
        public EGCEnabledBlackboardApi(ApplicationSettingsProvider settings, ILogger logger)
            : base(settings, logger)
        {
        }

        public IEnumerable<LmsQuizInfoDTO> GetItemsInfoForUser(LmsUserParameters lmsUserParameters, bool isSurvey, out string error)
        {
            var quizzes = this.GetItemsForUser(lmsUserParameters, isSurvey, null, out error);
            return quizzes.Select(q => new LmsQuizInfoDTO
            {
                id = q.id,
                name = q.title,
                course = q.course,
                courseName = q.courseName,
                lastModifiedLMS = q.lastModifiedLMS,
                isPublished = q.published
            });
        }

        public IEnumerable<LmsQuizDTO> GetItemsForUser(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds, out string error)
        {
            WebserviceWrapper client = null;

            var tests = this.LoginIfNecessary(
                ref client,
                c =>
                {
                    var egc = c.getEdugameCloudWrapper();
                    var content = c.getContentWrapper();

                    var tos = content.getTOCsByCourseId(lmsUserParameters.Course.ToString());

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

                        var loaded = content.loadFilteredContent(lmsUserParameters.Course.ToString(), contentFilter);
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
                                course = lmsUserParameters.Course,
                                courseName = lmsUserParameters.CourseName,
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
                            }
                            quizDTO.Add(lqd);
                        }

                        return quizDTO.ToList();
                    }

                    return new List<LmsQuizDTO> { };
                },
                lmsUserParameters.CompanyLms,
                    out error);

            return tests;
        }

        public void SendAnswers(LmsUserParameters lmsUserParameters, string contentId, bool isSurvey, string[] answers)
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
                            lmsUserParameters.Wstoken,
                            answers,
                            null,
                            isSurvey);
                    },
                lmsUserParameters.CompanyLms,
                out contentId);
        }

    }
}
