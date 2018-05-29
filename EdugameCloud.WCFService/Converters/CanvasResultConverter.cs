using System;
using SimpleJson;

namespace EdugameCloud.WCFService.Converters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;

    using RestSharp;

    public class CanvasResultConverter : QuizResultConverter
    {
        private readonly IEGCEnabledCanvasAPI _canvasApi;


        public CanvasResultConverter(IEGCEnabledCanvasAPI canvasApi)
        {
            _canvasApi = canvasApi;
        }


        public override async Task ConvertAndSendQuizResultToLmsAsync(IEnumerable<QuizQuestionResultDTO> results, QuizResult quizResult, LmsUserParameters lmsUserParameters)
        {
            var quizSubmission = _canvasApi.CreateQuizSubmission(
                lmsUserParameters.CompanyLms.LmsDomain,
                lmsUserParameters.LmsUser.Token,
                lmsUserParameters.Course,                
                quizResult.Quiz.LmsQuizId.GetValueOrDefault());

            foreach (var answer in results)
            {
                Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                if (question.LmsQuestionId == null)
                {
                    continue;
                }

                var answers = this.ProcessAnswers(question, answer);

                if (answers != null)
                {
                    quizSubmission.quiz_questions.Add(
                        new CanvasQuizSubmissionQuestionDTO {id = question.LmsQuestionId.Value, answer = answers});
                }
            }
            try
            {
                _canvasApi.AnswerQuestionsForQuiz(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    quizSubmission);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _canvasApi.CompleteQuizSubmission(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    lmsUserParameters.Course,
                    quizSubmission);
            }
        }

        public override async Task ConvertAndSendSurveyResultToLmsAsync(IEnumerable<SurveyQuestionResultDTO> results, 
            SurveyResult surveyResult, 
            LmsUserParameters lmsUserParameters)
        {
            var quizSubmission = _canvasApi.CreateQuizSubmission(
                lmsUserParameters.CompanyLms.LmsDomain,
                lmsUserParameters.LmsUser.Token,
                lmsUserParameters.Course,
                surveyResult.Survey.LmsSurveyId.GetValueOrDefault()
                );

            foreach (var answer in results)
            {
                Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                if (question.LmsQuestionId == null)
                {
                    continue;
                }

                var quizAnswer = new QuizQuestionResultDTO();
                quizAnswer.isCorrect = answer.isCorrect;
                if (question.QuestionType.Id == (int) QuestionTypeEnum.SingleMultipleChoiceText)
                {
                    quizAnswer.answers =
                        answer.answers.Where(a => a.surveyDistractorAnswerId != null)
                            .Select(a => a.surveyDistractorAnswerId.GetValueOrDefault().ToString())
                            .ToArray();
                }
                else
                {
                    quizAnswer.answers =
                        answer.answers.Where(a => a.value != null)
                            .Select(a => a.value)
                            .ToArray();
                }
                var answers = this.ProcessAnswers(question, quizAnswer);

                if (answers != null)
                {
                    quizSubmission.quiz_questions.Add(
                        new CanvasQuizSubmissionQuestionDTO {id = question.LmsQuestionId.Value, answer = answers});
                }
            }

            try
            {
                _canvasApi.AnswerQuestionsForQuiz(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    quizSubmission);
            }
            catch (Exception ex)
            {
                
            }
                finally
                {
                    _canvasApi.CompleteQuizSubmission(
                   lmsUserParameters.CompanyLms.LmsDomain,
                   lmsUserParameters.LmsUser.Token,
                   lmsUserParameters.Course,
                   quizSubmission);
                }
            
        }

        protected override string GetTrueFalseLmsIdAnswer(Question question, QuizQuestionResultDTO answer)
        {
            Distractor distractor = question.Distractors != null
                ? question.Distractors.FirstOrDefault(x => x.IsActive.GetValueOrDefault())
                : null;
            if (distractor != null)
            {
                bool answ;
                // TRICK: for survey using isCorrect logic is not valid
                // but only surveys have 'answers' property populated
                if ((answer.answers != null) && (answer.answers.Length > 0))
                {
                    answ = answer.answers[0] == "1";
                }
                else 
                {
                    answ = (answer.isCorrect && distractor.IsCorrect.GetValueOrDefault())
                        || (!answer.isCorrect && !distractor.IsCorrect.GetValueOrDefault());
                }

                return answ ? distractor.LmsAnswer : distractor.LmsAnswerId.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// The process answers.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object ProcessAnswers(Question question, QuizQuestionResultDTO answer)
        {
            object answers;

            switch (question.QuestionType.Id)
            {
                case (int)QuestionTypeEnum.TextNoQuestion:
                    return null;
                case (int)QuestionTypeEnum.TrueFalse:
                    answers = this.GetTrueFalseLmsIdAnswer(question, answer);
                    break;
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    var isSingle = this.IsSingleAnswer(question);
                    var answerIds = this.GetMultipleChoiceLmsIds(question, answer);

                    if (isSingle)
                    {
                        answers = answerIds.FirstOrDefault();
                    }
                    else
                    {
                        answers = answerIds;
                    }

                    break;
                case (int)QuestionTypeEnum.Matching:
                    {
                        var userAnswers = this.GetMatchingValues(question, answer, true);

                        var match = new List<JsonObject>();

                        foreach (var left in userAnswers.Keys)
                        {
                            var jsonObject = new JsonObject();
                            jsonObject.Add("answer_id", left);
                            jsonObject.Add("match_id", userAnswers[left]);
                            match.Add(jsonObject);
                        }

                        answers = match;
                        break;
                    }
                case (int)QuestionTypeEnum.MultipleDropdowns:
                    {
                        var obj = new JsonObject();
                        var userAnswers = this.GetMultipleDropdownValues(question, answer, false);
                        foreach (var key in userAnswers.Keys)
                        {
                            obj.Add(key, userAnswers[key]);
                        }
                        answers = obj;
                        break;
                    }

                case (int)QuestionTypeEnum.FillInTheBlank:
                    {
                        var obj = new JsonObject();
                        var userAnswers = this.GetMultipleBlanksValues(question, answer);
                        foreach (var key in userAnswers.Keys)
                        {
                            obj.Add(key, userAnswers[key]);
                        }

                        answers = obj;

                        break;
                    }
                default:
                    answers = answer.answers.FirstOrDefault();
                    break;
            }

            return answers;
        }
    }
}