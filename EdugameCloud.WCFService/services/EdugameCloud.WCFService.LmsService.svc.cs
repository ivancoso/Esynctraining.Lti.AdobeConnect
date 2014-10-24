// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Converters;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// The LMS service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LmsService : BaseService, ILmsService
    {
        #region Properties

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        private CompanyLmsModel CompanyLmsModel
        {
            get
            {
                return IoC.Resolve<CompanyLmsModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        /// Gets the quiz converter.
        /// </summary>
        private QuizConverter QuizConverter
        {
            get
            {
                return IoC.Resolve<QuizConverter>();
            }
        }

        #endregion

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
        /// The get quizzes for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms User Parameters Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<LmsQuizDTO> GetQuizzesForUser(int userId, int lmsUserParametersId)
        {
            var ret = new ServiceResponse<LmsQuizDTO>();

            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId);

            if (lmsUserParameters.Value != null)
            {
                var companyLms = lmsUserParameters.Value.CompanyLms;

                IEnumerable<LmsQuizDTO> quizzesForCourse = CourseAPI.GetQuizzesForCourse(
                    false, 
                    companyLms.LmsDomain, 
                    companyLms.AdminUser.Token, 
                    lmsUserParameters.Value.Course);

                ret.objects = quizzesForCourse;
            }
            else
            {
                ret.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Wrong id", "No lms user parameters found"));
            }            

            return ret;
        }

        /// <summary>
        /// The get authentication parameters by id.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<LmsUserParametersDTO> GetAuthenticationParametersById(LmsAuthenticationParametersDTO parameters)
        {
            var result = new ServiceResponse<LmsUserParametersDTO>();

            var param = this.LmsUserParametersModel.GetOneByAcId(parameters.acId).Value;
            result.@object = param != null ? new LmsUserParametersDTO(param) : null;
            
            return result;
        }

        /// <summary>
        /// The convert quizzes.
        /// </summary>
        /// <param name="quizzesInfo">
        /// The quizzes info.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizesAndSubModuleItemsDTO> ConvertQuizzes(LmsQuizConvertDTO quizzesInfo)
        {
            var serviceResponse = new ServiceResponse<QuizesAndSubModuleItemsDTO>();

            if (quizzesInfo.quizIds == null)
            {
                return serviceResponse;
            }
            
            var lmsUserParameters = LmsUserParametersModel.GetOneById(quizzesInfo.lmsUserParametersId);

            if (lmsUserParameters.Value != null)
            {
                var user = UserModel.GetOneById(quizzesInfo.userId);
                var companyLms = lmsUserParameters.Value.CompanyLms;


                IEnumerable<LmsQuizDTO> quizzes = CourseAPI.GetQuizzesForCourse(
                    true, 
                    companyLms.LmsDomain, 
                    companyLms.AdminUser.Token, 
                    lmsUserParameters.Value.Course);

                QuizConverter.ConvertQuizzes(quizzes, user.Value);

                //serviceResponse.objects = quizzesForCourse;
            }
            else
            {
                serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Wrong id", "No lms user parameters found"));
            }    
            
            return serviceResponse;
        }
        
        /*
        public void SaveAnswers()
        {
            List<LmsQuizDTO> quizzes = CourseAPI.GetQuizzesForCourse(true, "canvas.instructure.com", TeacherToken, 875207);

            foreach (LmsQuizDTO q in quizzes)
            {
                List<QuizSubmissionDTO> s = CourseAPI.GetSubmissionForQuiz(
                    "canvas.instructure.com", 
                    StudentToken, 
                    875207, 
                    q.id);
                foreach (QuizSubmissionDTO subm in s)
                {
                    // subm.access_code = StudentToken;
                    foreach (QuizQuestionDTO quest in q.questions.Take(2))
                    {
                        subm.quiz_questions.Add(new QuizSubmissionQuestionDTO { id = quest.id, answer = quest.answers.Last().id });
                    }

                    CourseAPI.AnswerQuestionsForQuiz("canvas.instructure.com", StudentToken, subm);
                    CourseAPI.ReturnSubmissionForQuiz("canvas.instructure.com", StudentToken, 875207, subm);
                }
            }
        }
        */
        #endregion
    }
}