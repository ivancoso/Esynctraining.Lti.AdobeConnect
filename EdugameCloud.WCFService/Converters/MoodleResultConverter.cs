using System;
using System.Collections.Generic;

namespace EdugameCloud.WCFService.Converters
{
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Utils;

    using RestSharp;

    public class MoodleResultConverter : QuizResultConverter
    {
        private IEGCEnabledMoodleApi MoodleApi
        {
            get
            {
                return IoC.Resolve<IEGCEnabledMoodleApi>();
            }
        }

        public override void ConvertAndSendQuizResultToLms(IEnumerable<QuizQuestionResultDTO> results, QuizResult quizResult, LmsUserParameters lmsUserParameters)
        {
            var toSend = new List<MoodleQuizResultDTO>();

            foreach (QuizQuestionResultDTO r in results)
            {
                var m = new MoodleQuizResultDTO();

                m.quizId = quizResult.Quiz.LmsQuizId ?? 0;
                Question question = this.QuestionModel.GetOneById(r.questionId).Value;
                m.questionId = question.LmsQuestionId ?? 0;
                m.questionType = question.QuestionType.Type;
                m.userId = lmsUserParameters.LmsUser.UserId;
                m.startTime = quizResult.StartTime.ConvertToUTCTimestamp();

                m.answers = this.ProcessAnswers(question, r);

                toSend.Add(m);
            }

            if (toSend.Count == 0)
            {
                return;
            }

            var ret =
                toSend.GroupBy(s => s.quizId)
                    .Select(
                        s =>
                        new
                        {
                            quizId = s.Key,
                            usersResults =
                        s.GroupBy(u => new { u.userId, u.startTime })
                        .Select(
                            u =>
                            new
                            {
                                u.Key.userId,
                                u.Key.startTime,
                                answers = u.Select(a => new { a.questionId, a.answers })
                            })
                        });

            string json = (new RestSharp.Serializers.JsonSerializer()).Serialize(ret);
            this.MoodleApi.SendAnswers(lmsUserParameters, json, false, null);
        }

        public override void ConvertAndSendSurveyResultToLms(IEnumerable<SurveyQuestionResultDTO> results, SurveyResult surveyResult, LmsUserParameters lmsUserParameters)
        {
            var toSend = new List<MoodleQuizResultDTO>();

            foreach (SurveyQuestionResultDTO r in results)
            {
                var m = new MoodleQuizResultDTO();

                m.quizId = surveyResult.Survey.LmsSurveyId ?? 0;
                Question question = this.QuestionModel.GetOneById(r.questionId).Value;
                m.questionId = question.LmsQuestionId ?? 0;
                m.questionType = question.QuestionType.Type;
                m.userId = lmsUserParameters.LmsUser.UserId;
                m.startTime = surveyResult.StartTime.ConvertToUTCTimestamp();

                m.answers = (question.QuestionType.Id == (int)QuestionTypeEnum.SingleMultipleChoiceText
                    || question.QuestionType.Id == (int)QuestionTypeEnum.Rate)
                ? this.GetSurveyAnswerLmsIds(question, r)
                : this.ConvertSurveyAnswersToQuizAnswers(question, r, false);

                toSend.Add(m);
            }

            if (toSend.Count == 0)
            {
                return;
            }

            var ret =
                toSend.GroupBy(s => s.quizId)
                    .Select(
                        s =>
                        new
                        {
                            surveyId = s.Key,
                            courseId = surveyResult.Survey.SubModuleItem.SubModuleCategory.LmsCourseId ?? 0,
                            usersResults =
                        s.GroupBy(u => new { u.userId, u.startTime })
                        .Select(
                            u =>
                            new
                            {
                                u.Key.userId,
                                u.Key.startTime,
                                answers = u.Select(a => new { a.questionId, answer = a.answers.Aggregate((b, c) => b.ToString() + "|" + c.ToString()) })
                            })
                        });

            string json = (new RestSharp.Serializers.JsonSerializer()).Serialize(ret);
            this.MoodleApi.SendAnswers(lmsUserParameters, json, true, null);
        }

        private object[] ProcessAnswers(Question question, QuizQuestionResultDTO answer)
        {
            switch (question.QuestionType.Id)
            {
                case (int)QuestionTypeEnum.TrueFalse:
                    return new[] { this.GetTrueFalseStringAnswer(question, answer) };
                case (int)QuestionTypeEnum.CalculatedMultichoice:
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    return this.GetMultipleChoiceLmsAnswers(question, answer).ToArray();
                case (int)QuestionTypeEnum.Matching:
                    var userAnswers = this.GetMatchingValues(question, answer, false);

                    var match = new List<JsonObject>();

                    foreach (var left in userAnswers.Keys)
                    {
                        var jsonObject = new JsonObject();
                        jsonObject.Add(left, userAnswers[left]);
                        match.Add(jsonObject);
                    }

                    return match.ToArray();
                default:
                    return answer.answers;
            }
        }
    }
}