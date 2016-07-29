using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace EdugameCloud.WCFService.Converters
{
    public class SakaiResultConverter : QuizResultConverter
    {
        private IEGCEnabledSakaiApi SakaiApi
        {
            get
            {
                return IoC.Resolve<IEGCEnabledSakaiApi>();
            }
        }


        public override void ConvertAndSendQuizResultToLms(IEnumerable<QuizQuestionResultDTO> results, QuizResult quizResult, LmsUserParameters lmsUserParameters)
        {
            Dictionary<int, string> answers = new Dictionary<int, string>();

            foreach (var answer in results)
            {
                Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                if (question.LmsQuestionId == null)
                {
                    continue;
                }

                var userAnswer = this.ProcessAnswers(question, answer);

                answers.Add(question.LmsQuestionId.GetValueOrDefault(), userAnswer);
            }

            var questions = this.QuestionModel.GetAllBySubModuleItemId(quizResult.Quiz.SubModuleItem.Id);

            var ret = new List<string>();
            foreach (var question in questions)
            {
                ret.Add(answers.ContainsKey(question.LmsQuestionId.GetValueOrDefault()) ? answers[question.LmsQuestionId.GetValueOrDefault()] : "0");
            }

            SakaiApi.SendAnswers(lmsUserParameters, quizResult.Quiz.LmsQuizId.GetValueOrDefault().ToString(), false, ret.ToArray());
        }

        public override void ConvertAndSendSurveyResultToLms(IEnumerable<SurveyQuestionResultDTO> results, SurveyResult surveyResult, LmsUserParameters lmsUserParameters)
        {
            Dictionary<int, string> answers = new Dictionary<int, string>();

            foreach (var answer in results)
            {
                Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                if (question.LmsQuestionId == null)
                {
                    continue;
                }

                QuizQuestionResultDTO quizAnswer = new QuizQuestionResultDTO();
                quizAnswer.isCorrect = answer.isCorrect;

                quizAnswer.answers = this.ConvertSurveyAnswersToQuizAnswers(
                    question,
                    answer,
                    question.QuestionType.Id == (int)QuestionTypeEnum.SingleMultipleChoiceText
                    || question.QuestionType.Id == (int)QuestionTypeEnum.RateScaleLikert);

                var userAnswer = this.ProcessAnswers(question, quizAnswer);
                var questionKey = question.LmsQuestionId.GetValueOrDefault();

                if (!answers.ContainsKey(questionKey))
                    answers.Add(questionKey, userAnswer);
            }

            var questions = this.QuestionModel.GetAllBySubModuleItemId(surveyResult.Survey.SubModuleItem.Id);

            var ret = new List<string>();
            foreach (var question in questions)
            {
                ret.Add(answers.ContainsKey(question.LmsQuestionId.GetValueOrDefault()) ? answers[question.LmsQuestionId.GetValueOrDefault()] : "0");
            }

            SakaiApi.SendAnswers(lmsUserParameters, surveyResult.Survey.LmsSurveyId.GetValueOrDefault().ToString(), true, ret.ToArray());
        }

        
        protected string GetMultipleChoiceAnswers(Question question, QuizQuestionResultDTO answer)
        {
            var answers = answer.answers ?? new string[] { };
            var distractorIds =
                question.Distractors.Select((distractor, index) => (answers.Contains(distractor.Id.ToString(CultureInfo.InvariantCulture)) && distractor.IsCorrect.HasValue &&  distractor.IsCorrect.Value)? index : -1 ).Where(x => x != -1);
            return string.Join(";", distractorIds);
        }

        /// <summary>
        /// The process blackboard answers.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessAnswers(Question question, QuizQuestionResultDTO answer)
        {
            switch (question.QuestionType.Id)
            {
                //case (int)QuestionTypeEnum.FillInTheBlank:
                //    {
                //        var obj = new JsonObject();
                //        var userAnswers = this.GetMultipleBlanksValues(question, answer);
                //        foreach (var key in userAnswers.Keys)
                //        {
                //            obj.Add(key, userAnswers[key]);
                //        }

                //        var answers = JsonConvert.SerializeObject(obj);

                //        return answers;
                //    }

                case (int)QuestionTypeEnum.TrueFalse:
                {
                    return this.GetTrueFalseStringAnswer(question, answer);
                }
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                {
                    //var isSingle = this.IsSingleAnswer(question);
                    //if (isSingle)
                    //{
                    //    return this.GetMultipleChoiceLmsIds(question, answer).FirstOrDefault().ToString();
                    //}
                    return this.GetMultipleChoiceAnswers(question, answer);
                }
                case (int)QuestionTypeEnum.RateScaleLikert:
                {
                    return this.GetMultipleChoiceLmsIds(question, answer).FirstOrDefault().ToString();
                }
                case (int)QuestionTypeEnum.Calculated:
                {
                    return string.Format("{0};{1}", question.Distractors.First().LmsAnswer, answer.answers.First());
                }
                case (int)QuestionTypeEnum.Hotspot:
                {
                    return answer.isCorrect ? question.Distractors.First().LmsAnswer : "0, 0";
                }
                case (int)QuestionTypeEnum.MultipleDropdowns:
                {
                    var userAnswers = this.GetMultipleDropdownValues(question, answer, true);
                    return string.Join(";", userAnswers.Keys.OrderBy(k => k).Select(k => userAnswers[k]));
                }
                case (int)QuestionTypeEnum.Sequence:
                {
                    var distractors = question.Distractors.OrderBy(d => d.DistractorOrder).ToList();
                    if (answer.isCorrect)
                    {
                        return string.Join(";", distractors.Select(d => d.LmsAnswerId.ToString()));
                    }
                    else
                    {
                        return string.Join(";", distractors.Select(d => (d.DistractorOrder - 1).ToString()));
                    }
                }
                case (int)QuestionTypeEnum.Matching:
                {
                    var userAnswers = this.GetMatchingValues(question, answer, true);
                    return string.Join(";", userAnswers.Keys.OrderBy(k => k).Select(k => userAnswers[k]));
                }
                default:
                {
                    return answer.answers != null
                        ? string.Join(";", answer.answers.Select(a => a ?? string.Empty))
                        : string.Empty;
                }
            }
        }
    }
}