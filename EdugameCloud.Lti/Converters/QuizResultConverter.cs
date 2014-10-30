namespace EdugameCloud.Lti.Converters
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Utils;

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

                        switch (question.QuestionType.Id)
                        {
                            case (int)QuestionTypeEnum.TrueFalse:
                                Distractor distractor = question.Distractors != null
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
