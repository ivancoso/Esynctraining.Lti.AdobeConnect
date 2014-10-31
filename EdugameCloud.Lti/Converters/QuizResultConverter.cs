namespace EdugameCloud.Lti.Converters
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Utils;

    using Newtonsoft.Json;

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
        /// The convert and send result.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        public void ConvertAndSendResult(IEnumerable<QuizQuestionResultDTO> results)
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

                        object answers = null;

                        Distractor distractor;

                        switch (question.QuestionType.Id)
                        {
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

                                    var answersDict =
                                        JsonConvert.DeserializeObject<List<KeyValuePair<string, int>>>(
                                            distractor.LmsAnswer) as List<KeyValuePair<string, int>>;
                                    
                                    JsonObject obj = new JsonObject();

                                    foreach (var key in answersDict)
                                    {
                                        var order = key.Value;
                                        if (answer.answers.Count > order)
                                        {
                                            var userText = answer.answers[order];
                                            /*
                                            var optionstart =
                                                distractor.DistractorName.IndexOf(
                                                    "name=\"" + userText + ""\,
                                                    System.StringComparison.Ordinal);

                                            if (optionstart > -1)
                                            {
                                                var text = distractor.DistractorName.Substring(optionstart);
                                                var 
                                            }
                                            */
                                            var xmlDoc = new XmlDocument();
                                            xmlDoc.LoadXml(distractor.DistractorName);
                                            var optionTag = xmlDoc.SelectSingleNode(
                                                string.Format("data//options//option[@name='{0}']", userText));
                                            int lmsId = 0;
                                            if (optionTag != null && optionTag.Attributes["lmsid"] != null)
                                            {
                                                lmsId = int.Parse(optionTag.Attributes["lmsid"].Value);
                                            }

                                            if (lmsId == 0)
                                            {
                                                obj.Add(key.Key, userText);
                                            }
                                            else
                                            {
                                                obj.Add(key.Key, lmsId);
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

                        submission.quiz_questions.Add(
                            new QuizSubmissionQuestionDTO
                            {
                                id = question.LmsQuestionId.Value,
                                answer = answers
                            });
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
    }
}
