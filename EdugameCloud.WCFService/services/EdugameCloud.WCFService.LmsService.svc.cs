// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Policy;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;

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
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions.AcceptanceCriteria;

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
        /// Gets the survey model.
        /// </summary>
        private SurveyModel SurveyModel
        {
            get
            {
                return IoC.Resolve<SurveyModel>();
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
        /// Gets the lms course meeting model.
        /// </summary>
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

        /// <summary>
        /// Gets the lms provider model.
        /// </summary>
        private LmsProviderModel LmsProviderModel
        {
            get
            {
                return IoC.Resolve<LmsProviderModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

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
        /// The get providers.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<LmsProviderDTO> GetProviders()
        {
            var result = new ServiceResponse<LmsProviderDTO>();

            var providers = LmsProviderModel.GetAll();
            result.objects = providers.Select(
                p =>
                    {
                        var pdto = new LmsProviderDTO(p);
                        pdto.configUrl = this.Settings.PortalUrl +
                            "content/lti-config/" +
                            p.ShortName + 
                            ".xml";
                        pdto.instructionsUrl = this.Settings.PortalUrl +
                            "content/lti-instructions/" +
                            p.ShortName + 
                            ".pdf";
                        return pdto;
                    });

            return result;
        }

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
            return this.GetItemsForUser(userId, lmsUserParametersId, false);
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
            return this.Convert(userId, lmsUserParametersId, quizIds, false) as ServiceResponse<QuizesAndSubModuleItemsDTO>;
        }

        /// <summary>
        /// The get surveys for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms user parameters id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<LmsQuizInfoDTO> GetSurveysForUser(int userId, int lmsUserParametersId)
        {
            return this.GetItemsForUser(userId, lmsUserParametersId, true);
        }

        /// <summary>
        /// The convert surveys.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms user parameters id.
        /// </param>
        /// <param name="quizIds">
        /// The quiz ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveysAndSubModuleItemsDTO> ConvertSurveys(
            int userId,
            int lmsUserParametersId,
            List<int> quizIds)
        {
            return this.Convert(userId, lmsUserParametersId, quizIds, true) as ServiceResponse<SurveysAndSubModuleItemsDTO>;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// The get items for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms user parameters id.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private ServiceResponse<LmsQuizInfoDTO> GetItemsForUser(int userId, int lmsUserParametersId, bool isSurvey)
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
                    null,
                    isSurvey);

                ret.objects = quizzesForCourse.Select(
                    q =>
                        {
                            int lastModified = 0;
                            if (isSurvey)
                            {
                                var egcSurvey = SurveyModel.GetOneByLmsSurveyId(user.Value.Id, q.id, (int)LmsProviderEnum.Canvas).Value;
                                lastModified = egcSurvey != null
                                                   ? egcSurvey.SubModuleItem.DateModified.ConvertToTimestamp()
                                                   : 0;
                            }
                            else
                            {
                                var egcQuiz = QuizModel.GetOneByLmsQuizId(user.Value.Id, q.id, (int)LmsProviderEnum.Canvas).Value;
                                lastModified = egcQuiz != null
                                                   ? egcQuiz.SubModuleItem.DateModified.ConvertToTimestamp()
                                                   : 0;
                            }
                        

                        return new LmsQuizInfoDTO()
                        {
                            id = q.id,
                            name = q.title,
                            course = course.id,
                            courseName = course.name,
                            lastModifiedEGC = lastModified,
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
        /// The convert quizzes.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms user parameters id.
        /// </param>
        /// <param name="quizIds">
        /// The quiz ids.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private ServiceResponse Convert(int userId, int lmsUserParametersId, List<int> quizIds, bool isSurvey)
        {
            if (quizIds == null)
            {
                return null;
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
                var token = lmsUserParameters.LmsUser.Return(
                    u => u.Token,
                    companyLms.AdminUser.Return(a => a.Token, string.Empty));

                IEnumerable<LmsQuizDTO> quizzes = CourseAPI.GetQuizzesForCourse(
                    true,
                    companyLms.LmsDomain,
                    token,
                    lmsUserParameters.Course,
                    quizIds,
                    isSurvey);

                var subModuleItemsQuizes = QuizConverter.ConvertQuizzes(quizzes, course, user, isSurvey);

                if (isSurvey)
                {
                    var serviceResponse = new ServiceResponse<SurveysAndSubModuleItemsDTO>();

                    var items = this.SubModuleItemModel.GetSurveySubModuleItemsByUserId(user.Id).ToList();
                    var surveys = this.SurveyModel.GetLmsSurveys(user.Id, lmsUserParameters.Course, companyLms.LmsProvider.Id);

                    serviceResponse.@object = new SurveysAndSubModuleItemsDTO
                    {
                        surveys = subModuleItemsQuizes.Select(x => surveys.FirstOrDefault(q => q.surveyId == x.Value)).ToList(),
                        subModuleItems = subModuleItemsQuizes.Select(x => items.FirstOrDefault(q => q.subModuleItemId == x.Key)).ToList(),
                    };
                    return serviceResponse;
                }
                else
                {
                    var serviceResponse = new ServiceResponse<QuizesAndSubModuleItemsDTO>();

                    var items = this.SubModuleItemModel.GetQuizSMItemsByUserId(user.Id).ToList();
                    var quizes = this.QuizModel.GetLMSQuizzes(user.Id, lmsUserParameters.Course, companyLms.LmsProvider.Id);

                    serviceResponse.@object = new QuizesAndSubModuleItemsDTO
                    {
                        quizzes = subModuleItemsQuizes.Select(x => quizes.FirstOrDefault(q => q.quizId == x.Value)).ToList(),
                        subModuleItems = subModuleItemsQuizes.Select(x => items.FirstOrDefault(q => q.subModuleItemId == x.Key)).ToList(),
                    };
                    return serviceResponse;
                }
            }
            else
            {
                var serviceResponse = new ServiceResponse<QuizesAndSubModuleItemsDTO>();


                serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Wrong id", "No lms user parameters found"));
                return serviceResponse;
            }

            return null;
        }

        #endregion
    }
}