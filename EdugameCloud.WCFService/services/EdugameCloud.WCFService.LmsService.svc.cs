// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.Base;

    /// <summary>
    /// The LMS service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LmsService : BaseService, ILmsService
    {
        #region Constants

        /// <summary>
        /// The student token.
        /// </summary>
        private const string StudentToken = "7~D43gwxvbSqjZPJMrmd4s9tmzIOAYDdNZPrVOiPurEoglmDcg5n7KL0wZyVE3sPH2";

        /// <summary>
        /// The teacher token.
        /// </summary>
        private const string TeacherToken = "7~pgFFIztorj5U9PBmBwQpxG2vVF3aCZtFDIsmDCCfxcXWmXGyyBiNUbBpEqsb447F";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert quizzes.
        /// </summary>
        public void ConvertQuizzes()
        {
            List<QuizDTO> quizzesForCourse = CourseAPI.GetQuizzesForCourse("canvas.instructure.com", TeacherToken, 865831);
        }

        /// <summary>
        /// The save answers.
        /// </summary>
        public void SaveAnswers()
        {
            List<QuizDTO> quizzes = CourseAPI.GetQuizzesForCourse("canvas.instructure.com", TeacherToken, 875207);

            foreach (QuizDTO q in quizzes)
            {
                List<QuizSubmissionDTO> s = CourseAPI.GetSubmissionForQuiz(
                    "canvas.instructure.com", 
                    StudentToken, 
                    875207, 
                    q.id);
                foreach (QuizSubmissionDTO subm in s)
                {
                    // subm.access_code = StudentToken;
                    foreach (QuizQuestionDTO quest in q.questions)
                    {
                        subm.quiz_questions.Add(new QuizSubmissionQuestionDTO { id = quest.id, answer = quest.answers.First().id });
                    }

                    CourseAPI.AnswerQuestionsForQuiz("canvas.instructure.com", StudentToken, subm);
                    CourseAPI.ReturnSubmissionForQuiz("canvas.instructure.com", StudentToken, 875207, subm);
                }
            }
        }

        #endregion
    }
}