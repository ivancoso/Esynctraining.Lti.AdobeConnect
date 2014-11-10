namespace EdugameCloud.Lti.Converters
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mime;
    using System.Xml;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Utils;

    using Newtonsoft.Json;

    using NHibernate.Hql.Ast.ANTLR;

    using RestSharp;
    using RestSharp.Deserializers;

    /// <summary>
    /// The quiz result converter.
    /// </summary>
    public class QuizResultConverter
    {
        #region Properties

        /// <summary>
        /// Gets the canvas course meeting model.
        /// </summary>
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        /// Gets the quiz result model.
        /// </summary>
        private QuizResultModel QuizResultModel
        {
            get
            {
                return IoC.Resolve<QuizResultModel>();
            }
        }

        /// <summary>
        /// Gets the survey result model.
        /// </summary>
        private SurveyResultModel SurveyResultModel
        {
            get
            {
                return IoC.Resolve<SurveyResultModel>();
            }
        }

        /// <summary>
        /// Gets the question model.
        /// </summary>
        private QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        /// <summary>
        /// Gets the course api.
        /// </summary>
        private CourseAPI CourseAPI
        {
            get
            {
                return IoC.Resolve<CourseAPI>();
            }
        }

        #endregion

        /// <summary>
        /// The convert and send survey result.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        public void ConvertAndSendSurveyResult(IEnumerable<SurveyQuestionResultDTO> results)
        {
            foreach (var userAnswer in results.GroupBy(r => r.surveyResultId))
            {
                var surveyResult = this.SurveyResultModel.GetOneById(userAnswer.Key).Value;
                if (surveyResult == null)
                {
                    continue;
                }

                var lmsUserParameters = surveyResult.LmsUserParameters;
                if (lmsUserParameters == null)
                {
                    return;
                }

                var lmsQuizId = surveyResult.Survey.LmsSurveyId;
                if (lmsQuizId == null)
                {
                    continue;
                }

                var quizSubmissions = CourseAPI.GetSubmissionForQuiz(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    lmsUserParameters.Course,
                    lmsQuizId.Value);

                foreach (var submission in quizSubmissions)
                {
                    foreach (var answer in userAnswer)
                    {
                        Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                        if (question.LmsQuestionId == null)
                        {
                            continue;
                        }

                        QuizQuestionResultDTO quizAnswer = new QuizQuestionResultDTO();
                        quizAnswer.isCorrect = answer.isCorrect;
                        if (question.QuestionType.Id == (int)QuestionTypeEnum.SingleMultipleChoiceText)
                        {
                            quizAnswer.answers =
                                answer.answers.Where(a => a.surveyDistractorAnswerId != null)
                                    .Select(a => a.surveyDistractorAnswerId.GetValueOrDefault().ToString())
                                    .ToList();
                        }
                        else
                        {
                            quizAnswer.answers =
                                answer.answers.Where(a => a.value != null)
                                    .Select(a => a.value)
                                    .ToList();
                        }
                        var answers = this.ProcessAnswers(question, quizAnswer);

                        if (answers != null)
                        {
                            submission.quiz_questions.Add(
                                new QuizSubmissionQuestionDTO { id = question.LmsQuestionId.Value, answer = answers });
                        }
                    }

                    CourseAPI.AnswerQuestionsForQuiz(
                        lmsUserParameters.CompanyLms.LmsDomain,
                        lmsUserParameters.LmsUser.Token,
                        submission);

                    CourseAPI.ReturnSubmissionForQuiz(
                        lmsUserParameters.CompanyLms.LmsDomain,
                        lmsUserParameters.LmsUser.Token,
                        lmsUserParameters.Course,
                        submission);
                }
            }   
        }

        /// <summary>
        /// The convert and send result.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        public void ConvertAndSendQuizResult(IEnumerable<QuizQuestionResultDTO> results)
        {
            foreach (var userAnswer in results.GroupBy(r => r.quizResultId))
            {
                var quizResult = this.QuizResultModel.GetOneById(userAnswer.Key).Value;
                if (quizResult == null)
                {
                    continue;
                }

                var lmsUserParameters = quizResult.LmsUserParameters;
                if (lmsUserParameters == null)
                {
                    return;
                }

                var lmsQuizId = quizResult.Quiz.LmsQuizId;
                if (lmsQuizId == null)
                {
                    continue;
                }

                var quizSubmissions = CourseAPI.GetSubmissionForQuiz(
                    lmsUserParameters.CompanyLms.LmsDomain, 
                    lmsUserParameters.LmsUser.Token, 
                    lmsUserParameters.Course, 
                    lmsQuizId.Value);

                foreach (var submission in quizSubmissions)
                {
                    foreach (var answer in userAnswer)
                    {
                        Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                        if (question.LmsQuestionId == null)
                        {
                            continue;
                        }

                        var answers = this.ProcessAnswers(question, answer);

                        if (answers != null)
                        {
                            submission.quiz_questions.Add(
                                new QuizSubmissionQuestionDTO { id = question.LmsQuestionId.Value, answer = answers });
                        }
                    }

                    CourseAPI.AnswerQuestionsForQuiz(
                        lmsUserParameters.CompanyLms.LmsDomain,
                        lmsUserParameters.LmsUser.Token, 
                        submission);

                    CourseAPI.ReturnSubmissionForQuiz(
                        lmsUserParameters.CompanyLms.LmsDomain,
                        lmsUserParameters.LmsUser.Token,
                        lmsUserParameters.Course,
                        submission);
                }
            }   
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
            object answers = null;

            Distractor distractor;

            switch (question.QuestionType.Id)
            {
                case (int)QuestionTypeEnum.TextNoQuestion:
                    return null;
                case (int)QuestionTypeEnum.TrueFalse:
                    distractor = question.Distractors != null
                                        ? question.Distractors.FirstOrDefault()
                                        : null;

                    if (distractor != null)
                    {
                        bool tfAnswer = (answer.isCorrect && distractor.IsCorrect.GetValueOrDefault())
                                       || (!answer.isCorrect && !distractor.IsCorrect.GetValueOrDefault());
                        answers = tfAnswer ? distractor.LmsAnswer : distractor.LmsAnswerId.ToString();
                    }

                    break;
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    var multAnswers =
                        question.Distractors.Where(q => answer.answers != null && answer.answers.Contains(q.Id.ToString(CultureInfo.InvariantCulture)))
                            .Select(q => q.LmsAnswerId)
                            .ToList();

                    if (question.IsMoodleSingle.GetValueOrDefault())
                    {
                        answers = multAnswers.FirstOrDefault();
                    }
                    else
                    {
                        answers = multAnswers;
                    }

                    break;
                case (int)QuestionTypeEnum.Matching:
                    {
                        var userAnswers = new Dictionary<string, string>();
                        if (answer.answers != null)
                        {
                            answer.answers.ForEach(
                                a =>
                                {
                                    int splitInd = a.IndexOf("$$", System.StringComparison.Ordinal);
                                    if (splitInd > -1)
                                    {
                                        string left = a.Substring(0, splitInd),
                                               right = a.Substring(splitInd + 2, a.Length - splitInd - 2);
                                        if (!userAnswers.ContainsKey(left))
                                        {
                                            userAnswers.Add(left, right);
                                        }
                                    }
                                });
                        }

                        var match = new List<JsonObject>();

                        foreach (var left in userAnswers.Keys)
                        {
                            var right = userAnswers[left];
                            var distactorLeft =
                                question.Distractors.FirstOrDefault(
                                    d => d.DistractorName.StartsWith(left + "$$"));

                            var distactorRight =
                                question.Distractors.FirstOrDefault(
                                    d => d.DistractorName.EndsWith("$$" + right));
                            if (distactorLeft != null && distactorRight != null)
                            {
                                var jsonObject = new JsonObject();
                                jsonObject.Add("answer_id", distactorLeft.LmsAnswerId);
                                jsonObject.Add("match_id", distactorRight.LmsAnswer);
                                match.Add(jsonObject);
                            }
                        }

                        answers = match;
                        break;
                    }
                case (int)QuestionTypeEnum.MultipleDropdowns:
                case (int)QuestionTypeEnum.FillInTheBlank:
                    {
                        distractor = question.Distractors != null
                                        ? question.Distractors.FirstOrDefault()
                                        : null;

                        if (distractor == null || distractor.LmsAnswer == null)
                        {
                            break;
                        }

                        Dictionary<string, int> answersDict =
                            JsonConvert.DeserializeObject<Dictionary<string, int>>(distractor.LmsAnswer);

                        JsonObject obj = new JsonObject();

                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(distractor.DistractorName);
                        var textTags = xmlDoc.SelectNodes(
                            string.Format("data//text[@isBlank='true']"));

                        foreach (var key in answersDict.Keys)
                        {
                            var order = answersDict[key];
                            if (answer.answers.Count > order)
                            {
                                var userText = answer.answers[order];

                                if (question.QuestionType.Id == (int)QuestionTypeEnum.FillInTheBlank)
                                {
                                    obj.Add(key, userText);
                                }
                                else
                                {
                                    int lmsId = 0;
                                    var textTag = textTags.Count > order ? textTags[order] : null;
                                    if (textTag != null)
                                    {
                                        var id = textTag.Attributes["id"].Value;
                                        var optionTag =
                                            xmlDoc.SelectSingleNode(
                                                string.Format("data//options[@id='{0}']//option[@name='{1}']",
                                                id,
                                                userText));
                                        if (optionTag != null && optionTag.Attributes["lmsid"] != null)
                                        {
                                            lmsId = int.Parse(optionTag.Attributes["lmsid"].Value);
                                        }
                                    }

                                    if (lmsId > 0)
                                        obj.Add(key, lmsId);
                                }
                            }
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
