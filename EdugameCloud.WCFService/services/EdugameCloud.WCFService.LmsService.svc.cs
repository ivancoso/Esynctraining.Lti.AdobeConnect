// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.Base;

    using Weborb.Reader;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
    IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LmsService : BaseService, ILmsService
    {
        private const string teacherToken = "7~pgFFIztorj5U9PBmBwQpxG2vVF3aCZtFDIsmDCCfxcXWmXGyyBiNUbBpEqsb447F";

        private const string studentToken = "7~D43gwxvbSqjZPJMrmd4s9tmzIOAYDdNZPrVOiPurEoglmDcg5n7KL0wZyVE3sPH2";

        public void ConvertQuizzes()
        {
            var q = CourseAPI.GetQuizzesForCourse("canvas.instructure.com", 
                teacherToken,                
                865831);


        }

        public void SaveAnswers()
        {

            var quizzes = CourseAPI.GetQuizzesForCourse("canvas.instructure.com",
                teacherToken,
                875207);

            foreach (var q in quizzes)
            {
                var s = CourseAPI.GetSubmissionForQuiz(
                    "canvas.instructure.com",
                    studentToken,
                    875207,
                    q.id);
                foreach (var subm in s)
                {
                    //subm.access_code = studentToken;
                    foreach (var quest in q.questions)
                    {
                        subm.quiz_questions.Add(new QuizSubmissionQuestionDTO(){ id = quest.id, answer = quest.answers.First().id });
                    }
                    CourseAPI.AnswerQuestionsForQuiz("canvas.instructure.com", studentToken, subm);
                    CourseAPI.ReturnSubmissionForQuiz("canvas.instructure.com", studentToken, 875207, subm);
                }
            }
        }
    }
}
