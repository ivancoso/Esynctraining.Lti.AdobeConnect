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
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Converters;
    using EdugameCloud.Lti.DTO;
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
        /// Gets the quiz model.
        /// </summary>
        private QuizModel QuizModel
        {
            get
            {
                return IoC.Resolve<QuizModel>();
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

        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get
            {
                return IoC.Resolve<LmsCourseMeetingModel>();
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
        public ServiceResponse<LmsQuizInfoDTO> GetQuizzesForUser(int userId, int lmsUserParametersId)
        {
            var ret = new ServiceResponse<LmsQuizInfoDTO>();

            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId);

            if (lmsUserParameters.Value != null)
            {
                var user = UserModel.GetOneById(userId);
                var companyLms = lmsUserParameters.Value.CompanyLms;

                var course = CourseAPI.GetCourse(
                    companyLms.LmsDomain,
                    lmsUserParameters.Value.LmsUser.Token,
                    lmsUserParameters.Value.Course);
                
                IEnumerable<LmsQuizDTO> quizzesForCourse = CourseAPI.GetQuizzesForCourse(
                    false, 
                    companyLms.LmsDomain, 
                    lmsUserParameters.Value.LmsUser.Token, 
                    lmsUserParameters.Value.Course,
                    null);

                ret.objects = quizzesForCourse.Select(
                    q =>
                        {
                            var egcQuiz = QuizModel.GetOneByLmsQuizId(user.Value.Id, q.id, (int)LmsProviderEnum.Canvas).Value;

                            return new LmsQuizInfoDTO()
                                       {
                                           id = q.id, 
                                           name = q.title,
                                           course = course.id,
                                           courseName = course.name,
                                           lastModifiedEGC = egcQuiz != null ? egcQuiz.SubModuleItem.DateModified.ConvertToTimestamp() : 0,
                                           isPublished = q.published
                                       };
                        });
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
        /// <param name="acId">
        /// The ac id.
        /// </param>
        /// <param name="acDomain">
        /// The ac domain.
        /// </param>
        /// <param name="scoId">
        /// The sco id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<LmsUserParametersDTO> GetAuthenticationParametersById(string acId, string acDomain, string scoId)
        {
            var result = new ServiceResponse<LmsUserParametersDTO>();

            var companyLms = CompanyLmsModel.GetOneByAcDomain(acDomain).Value;
            if (companyLms == null)
            {
                result.SetError(new Error(
                    Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "No AC Server found", "No company has set this ac domain as theirs"));
                return result;
            }

            var courseMeeting = LmsCourseMeetingModel.GetOneByMeetingId(companyLms.Id, scoId).Value;

            if (courseMeeting == null)
            {
                result.SetError(new Error(
                    Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "No course found", "This meeting is not associated to any course"));
                return result;
            }

            var param = this.LmsUserParametersModel.GetOneForLogin(acId, acDomain, courseMeeting.CourseId).Value;
            result.@object = param != null ? new LmsUserParametersDTO(param) : null;
            
            return result;
        }

        /// <summary>
        /// The convert quizzes.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms User Parameters Id.
        /// </param>
        /// <param name="quizIds">
        /// The quiz Ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizesAndSubModuleItemsDTO> ConvertQuizzes(int userId, int lmsUserParametersId, List<int> quizIds)
        {
            var serviceResponse = new ServiceResponse<QuizesAndSubModuleItemsDTO>();

            if (quizIds == null)
            {
                return serviceResponse;
            }
            
            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;

            if (lmsUserParameters != null)
            {
                var user = UserModel.GetOneById(userId).Value;
                var companyLms = lmsUserParameters.CompanyLms;
                var course = CourseAPI.GetCourse(
                    companyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    lmsUserParameters.Course);

                IEnumerable<LmsQuizDTO> quizzes = CourseAPI.GetQuizzesForCourse(
                    true, 
                    companyLms.LmsDomain, 
                    companyLms.AdminUser.Token, 
                    lmsUserParameters.Course,
                    quizIds);
                
                var subModuleItemsQuizes = QuizConverter.ConvertQuizzes(quizzes, course, user);

                var items = this.SubModuleItemModel.GetQuizSMItemsByUserId(user.Id).ToList();
                var quizes = this.QuizModel.GetLMSQuizzes(user.Id, lmsUserParameters.Course, companyLms.LmsProvider.Id);

                serviceResponse.@object = new QuizesAndSubModuleItemsDTO
                {
                    quizzes = subModuleItemsQuizes.Select(x => quizes.FirstOrDefault(q => q.quizId == x.Value)).ToList(),
                    subModuleItems = subModuleItemsQuizes.Select(x => items.FirstOrDefault(q => q.subModuleItemId == x.Key)).ToList(),
                };
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