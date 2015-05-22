﻿// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Converters;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;
    using ILmsService = EdugameCloud.WCFService.Contracts.ILmsService;

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
        /// Gets the meeting setup.
        /// </summary>
        private MeetingSetup MeetingSetup
        {
            get
            {
                return IoC.Resolve<MeetingSetup>();
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
        /// Gets the LMS factory.
        /// </summary>
        private LmsFactory LmsFactory
        {
            get
            {
                return IoC.Resolve<LmsFactory>();
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
        /// Gets the LMS provider model.
        /// </summary>
        private LmsProviderModel LmsProviderModel
        {
            get
            {
                return IoC.Resolve<LmsProviderModel>();
            }
        }

        private LmsCompanyRoleMappingModel LmsCompanyRoleMappingModel
        {
            get
            {
                return IoC.Resolve<LmsCompanyRoleMappingModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get authentication parameters by id.
        /// </summary>
        /// <param name="acId">
        /// The AC id.
        /// </param>
        /// <param name="acDomain">
        /// The AC domain.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserParametersDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public LmsUserParametersDTO GetAuthenticationParametersById(string acId, string acDomain, string scoId)
        {
            string errorString = null;
            var param = this.MeetingSetup.GetLmsParameters(acId, acDomain, scoId, ref errorString);
            if (param != null)
            {
                return param;
            }

            var error = new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "No parameters found", errorString);
            this.LogError("LMS.GetAuthenticationParametersById", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get providers.
        /// </summary>
        /// <returns>
        /// The <see cref="LmsProviderDTO"/>.
        /// </returns>
        public LmsProviderDTO[] GetProviders()
        {
            var providers = LmsProviderModel.GetAll();
            return providers.Select(
                p =>
                    {
                        var pdto = new LmsProviderDTO(p)
                        {
                            configUrl =
                            string.IsNullOrWhiteSpace(p.ConfigurationUrl)
                            ? string.Format("{0}content/lti-config/{1}.xml", (string)this.Settings.PortalUrl, p.ShortName)
                            : p.ConfigurationUrl,

                            instructionsUrl = string.Format("{0}content/lti-instructions/{1}.pdf", (string)this.Settings.PortalUrl, p.ShortName),
                            defaultRoleMapping = LmsCompanyRoleMappingModel.GetDefaultMapping(p.Id).ToArray(),
                        };

                        pdto.nameWithoutSpaces = pdto.lmsProviderName.Replace(" ", string.Empty);
                        return pdto;
                    }).ToArray();
        }

        /// <summary>
        /// The get quizzes for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS User Parameters Id.
        /// </param>
        /// <returns>
        /// The <see cref="LmsQuizInfoDTO"/>.
        /// </returns>
        public LmsQuizInfoDTO[] GetQuizzesForUser(int userId, int lmsUserParametersId)
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
        /// The LMS User Parameters Id.
        /// </param>
        /// <param name="quizIds">
        /// The quiz Ids.
        /// </param>
        /// <returns>
        /// The <see cref="QuizesAndSubModuleItemsDTO"/>.
        /// </returns>
        public QuizesAndSubModuleItemsDTO ConvertQuizzes(int userId, int lmsUserParametersId, int[] quizIds)
        {
            quizIds = quizIds ?? new int[0];
            return this.Convert(userId, lmsUserParametersId, quizIds, false) as QuizesAndSubModuleItemsDTO;
        }

        /// <summary>
        /// The get surveys for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS user parameters id.
        /// </param>
        /// <returns>
        /// The <see cref="LmsQuizInfoDTO"/>.
        /// </returns>
        public LmsQuizInfoDTO[] GetSurveysForUser(int userId, int lmsUserParametersId)
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
        /// The LMS user parameters id.
        /// </param>
        /// <param name="quizIds">
        /// The quiz ids.
        /// </param>
        /// <returns>
        /// The <see cref="SurveysAndSubModuleItemsDTO"/>.
        /// </returns>
        public SurveysAndSubModuleItemsDTO ConvertSurveys(
            int userId,
            int lmsUserParametersId,
            int[] quizIds)
        {
            quizIds = quizIds ?? new int[0];
            return this.Convert(userId, lmsUserParametersId, quizIds, true) as SurveysAndSubModuleItemsDTO;
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
        /// The LMS user parameters id.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <returns>
        /// The <see cref="LmsQuizInfoDTO"/>.
        /// </returns>
        private LmsQuizInfoDTO[] GetItemsForUser(int userId, int lmsUserParametersId, bool isSurvey)
        {
            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;
            Error error;
            if (lmsUserParameters != null)
            {
                var user = UserModel.GetOneById(userId);

                string errorString;
                var quizzesForCourse =
                    LmsFactory.GetEGCEnabledLmsAPI((LmsProviderEnum)lmsUserParameters.CompanyLms.LmsProvider.Id)
                        .GetItemsInfoForUser(lmsUserParameters, isSurvey, out errorString)
                        .ToList();
                if (string.IsNullOrWhiteSpace(errorString))
                {
                    quizzesForCourse.ForEach(
                        q =>
                            {
                                int lastModified;
                                if (isSurvey)
                                {
                                    var egcSurvey =
                                        SurveyModel.GetOneByLmsSurveyId(
                                            user.Value.Id,
                                            q.id,
                                            lmsUserParameters.CompanyLms.Id).Value;
                                    lastModified = egcSurvey != null
                                                       ? egcSurvey.SubModuleItem.DateModified.ConvertToTimestamp()
                                                       : 0;
                                }
                                else
                                {
                                    var egcQuiz =
                                        QuizModel.GetOneByLmsQuizId(
                                            user.Value.Id,
                                            q.id,
                                            lmsUserParameters.CompanyLms.Id).Value;
                                    lastModified = egcQuiz != null
                                                       ? egcQuiz.SubModuleItem.DateModified.ConvertToTimestamp()
                                                       : 0;
                                }

                                q.lastModifiedEGC = lastModified;
                            });

                    return quizzesForCourse.ToArray();
                }

                error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "Wrong response", errorString);
            }
            else
            {
                error = new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Wrong id", "No lms user parameters found");
            }

            this.LogError("AppletResult.GetById", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The convert quizzes.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS user parameters id.
        /// </param>
        /// <param name="quizIds">
        /// The quiz ids.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object Convert(int userId, int lmsUserParametersId, IEnumerable<int> quizIds, bool isSurvey)
        {
            if (quizIds == null)
            {
                return null;
            }

            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;

            if (lmsUserParameters != null)
            {
                var user = UserModel.GetOneById(userId).Value;
                if (user == null)
                {
                    var err = new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Wrong id", "No user found");
                    this.LogError("LMS.Convert", err);
                    throw new FaultException<Error>(err, err.errorMessage);
                }

                var companyLms = lmsUserParameters.CompanyLms;
                
                string error;
                IEnumerable<LmsQuizDTO> quizzes = LmsFactory.GetEGCEnabledLmsAPI((LmsProviderEnum)companyLms.LmsProvider.Id)
                    .GetItemsForUser(
                        lmsUserParameters,
                        isSurvey,
                        quizIds,
                        out error);

                var subModuleItemsQuizes = QuizConverter.ConvertQuizzes(quizzes, user, isSurvey, lmsUserParameters.CompanyLms.Id);

                if (isSurvey)
                {
                    var items = this.SubModuleItemModel.GetSurveySubModuleItemsByUserId(user.Id).ToList();
                    var surveys = this.SurveyModel.GetLmsSurveys(user.Id, lmsUserParameters.Course, companyLms.Id);

                    return new SurveysAndSubModuleItemsDTO
                    {
                        surveys = subModuleItemsQuizes.Select(x => surveys.FirstOrDefault(q => q.surveyId == x.Value)).ToArray(),
                        subModuleItems = subModuleItemsQuizes.Select(x => items.FirstOrDefault(q => q.subModuleItemId == x.Key)).ToArray(),
                    };
                }
                else
                {
                    var items = this.SubModuleItemModel.GetQuizSMItemsByUserId(user.Id).ToList();
                    var quizes = this.QuizModel.GetLMSQuizzes(user.Id, lmsUserParameters.Course, companyLms.Id);

                    return new QuizesAndSubModuleItemsDTO
                    {
                        quizzes = subModuleItemsQuizes.Select(x => quizes.FirstOrDefault(q => q.quizId == x.Value))
                        .ToList().Select(x => new QuizFromStoredProcedureDTO(x)).ToArray(),
                        subModuleItems = subModuleItemsQuizes.Select(x => items.FirstOrDefault(q => q.subModuleItemId == x.Key)).ToArray(),
                    };
                }
            }
            else
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Wrong id", "No lms user parameters found");
                this.LogError("LMS.Convert", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }
        }

        #endregion

    }

}