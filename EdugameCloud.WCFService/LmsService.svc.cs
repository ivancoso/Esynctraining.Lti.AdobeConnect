using System.ServiceModel.Channels;
using System.Web.Hosting;

namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;
    using EdugameCloud.Core;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Core;
    using EdugameCloud.Lti.Core.Business.MeetingNameFormatting;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.DTO;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Converters;
    using EdugameCloud.WCFService.DTO;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AdobeConnect;
    using Esynctraining.Core;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using Resources;
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

        private QuizModel QuizModel => IoC.Resolve<QuizModel>();

        private SurveyModel SurveyModel => IoC.Resolve<SurveyModel>();

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();

        private LmsUserParametersModel LmsUserParametersModel => IoC.Resolve<LmsUserParametersModel>();

        private LmsFactory LmsFactory => IoC.Resolve<LmsFactory>();

        private QuizConverter QuizConverter => IoC.Resolve<QuizConverter>();

        private LmsProviderModel LmsProviderModel => IoC.Resolve<LmsProviderModel>();

        private LmsCompanyModel LmsCompanyModel => IoC.Resolve<LmsCompanyModel>();

        private LmsCompanyRoleMappingModel LmsCompanyRoleMappingModel => IoC.Resolve<LmsCompanyRoleMappingModel>();

        private Lti.API.AdobeConnect.IAdobeConnectAccountService AdobeConnectAccountService => IoC.Resolve<Lti.API.AdobeConnect.IAdobeConnectAccountService>();

        private IMeetingNameFormatterFactory MeetingNameFormatterFactory => IoC.Resolve<IMeetingNameFormatterFactory>();
        private IBuildVersionProcessor VersionProcessor => IoC.Resolve<IBuildVersionProcessor>();

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
                            configUrl = string.IsNullOrWhiteSpace(p.ConfigurationUrl)
                                ? new Uri(new Uri((string)Settings.PortalUrl, UriKind.Absolute), $"content/lti-config/{p.LmsProviderName}.xml").ToString()
                                : p.ConfigurationUrl,

                            instructionsUrl = new Uri(new Uri((string)Settings.PortalUrl, UriKind.Absolute), $"content/lti-instructions/{p.LmsProviderName}.pdf").ToString(),

                            defaultRoleMapping = LmsCompanyRoleMappingModel.GetDefaultMapping(p.Id).ToArray(),
                        };

                        pdto.nameWithoutSpaces = pdto.lmsProviderName.Replace(" ", string.Empty);
                        return pdto;
                    }).ToArray();
        }

        public FileDownloadDTO[] GetFiles(int lmsProviderId)
        {
            switch (lmsProviderId)
            {
                case (int)LmsProviderEnum.Blackboard:
                    return new FileDownloadDTO[]
                    {
                        BuildUserGuide(LmsProviderEnum.Blackboard),
                        BuildMobileDownload(),
                        BuildOfficeHoursePod(),
                        BuildBlackboardJar(),
                    };

                case (int)LmsProviderEnum.Moodle:
                    return new FileDownloadDTO[]
                    {
                        BuildUserGuide(LmsProviderEnum.Moodle),
                        BuildMobileDownload(),
                        BuildOfficeHoursePod(),
                        BuildMoodleZip(),
                    };

                case (int)LmsProviderEnum.Canvas:
                    return new FileDownloadDTO[]
                    {
                        BuildUserGuide(LmsProviderEnum.Canvas),
                        BuildMobileDownload(),
                        BuildOfficeHoursePod(),
                    };

                case (int)LmsProviderEnum.AgilixBuzz:
                    return new FileDownloadDTO[]
                    {
                        BuildUserGuide(LmsProviderEnum.AgilixBuzz),
                        BuildMobileDownload(),
                        BuildOfficeHoursePod(),
                    };

                case (int)LmsProviderEnum.Schoology:
                    return new FileDownloadDTO[]
                    {
                        BuildUserGuide(LmsProviderEnum.Schoology),
                        BuildMobileDownload(),
                        BuildOfficeHoursePod(),
                    };

                case (int)LmsProviderEnum.Brightspace:
                    return new FileDownloadDTO[]
                    {
                        BuildUserGuide(LmsProviderEnum.Brightspace),
                        BuildMobileDownload(),
                        BuildOfficeHoursePod(),
                    };

                case (int)LmsProviderEnum.Sakai:
                    return new FileDownloadDTO[]
                    {
                        BuildUserGuide(LmsProviderEnum.Sakai),
                        BuildMobileDownload(),
                        BuildOfficeHoursePod(),
                    };
            }

            var error = new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Invalid lmsProviderId", "Not supported LMS.");
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public IdNamePairDTO[] GetMeetingNameFormatters() 
        {
            return MeetingNameFormatterFactory.GetFormatters().Select(x => new IdNamePairDTO 
            {
                Id = x.Key,
                Name = x.Value,
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

        public PrincipalReportDto[] GetMeetingHostReport(int lmsCompanyId)
        {
            LmsCompany license = this.LmsCompanyModel.GetOneById(lmsCompanyId).Value;

            var provider = AdobeConnectAccountService.GetProvider(license);

            return AdobeConnectAccountService.GetMeetingHostReport(provider).ToArray();
        }

        public OperationResultDto DeletePrincipals(int lmsCompanyId, string login, string password, string[] principalIds)
        {
            //http://dev.connectextensions.com/api/xml?action=principal-list&filter-principal-id=313091&filter-principal-id=256215&filter-principal-id=257331
            try
            {
                if (principalIds == null)
                    throw new ArgumentNullException("principalIds");

                LmsCompany currentLicense = this.LmsCompanyModel.GetOneById(lmsCompanyId).Value;
                IAdobeConnectProxy currentLicenseProvider = null;
                try
                {
                    currentLicenseProvider = AdobeConnectAccountService.GetProvider(currentLicense.AcServer, new UserCredentials(login, password), true);
                }
                catch (InvalidOperationException)
                {
                    return OperationResultDto.Error("Login to Adobe Connect failed.");
                }
                PrincipalCollectionResult principalsToDelete = currentLicenseProvider.GetAllByPrincipalIds(principalIds);

                IEnumerable<LmsCompany> companyLicenses = this.LmsCompanyModel.GetAllByCompanyId(currentLicense.CompanyId);
                var lmsLicensePrincipals = new List<string>();
                foreach (LmsCompany lms in companyLicenses)
                {
                    if (lms.AcServer.TrimEnd(new char[] { '/' }) == currentLicense.AcServer.TrimEnd(new char[] { '/' }))
                    {
                        bool tryToDeleteAcUserFromLicense = principalsToDelete.Values.Select(x => x.Login).Contains(lms.AcUsername);
                        if (tryToDeleteAcUserFromLicense)
                            lmsLicensePrincipals.Add(string.Format("Adobe Connect account '{0}' is used within your LMS license '{1}'. ", lms.AcUsername, lms.Title));
                    }
                }

                if (lmsLicensePrincipals.Count > 0)
                {
                    string msg = (lmsLicensePrincipals.Count == 1)
                        ? "You should not delete account. "
                        : "You should not delete some accounts. ";
                    return OperationResultDto.Error(msg + string.Join("", lmsLicensePrincipals));
                }

                bool allOK = true;
                var failedPrincipals = new List<string>();

                foreach (string principalId in principalIds)
                {
                    PrincipalResult deleteResult = currentLicenseProvider.PrincipalDelete(new PrincipalDelete { PrincipalId = principalId });
                    if (deleteResult.Status.Code != StatusCodes.ok)
                    {
                        Logger.ErrorFormat("AC.PrincipalDelete error. {0} PrincipalId: {1}.", 
                            deleteResult.Status.GetErrorInfo(),
                            principalId);
                        allOK = false;
                        failedPrincipals.Add(principalId);
                    }
                }

                if (allOK)
                    return OperationResultDto.Success();
                else
                    return OperationResultDto.Error(string.Format("Failed to delete {0} principal(s) from Adobe Connect", failedPrincipals.Count.ToString()));
            }
            catch (Exception ex)
            {
                Logger.Error("LmsService.DeletePrincipals", ex);
                return OperationResultDto.Error(ErrorsTexts.UnexpectedError);
            }
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
                    LmsFactory.GetEGCEnabledLmsAPI((LmsProviderEnum)lmsUserParameters.CompanyLms.LmsProviderId)
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

            try
            {
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
                    IEnumerable<LmsQuizDTO> quizzes = LmsFactory.GetEGCEnabledLmsAPI((LmsProviderEnum)companyLms.LmsProviderId)
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
            catch (Exception ex)
            {
                if (ex is IUserMessageException)
                {
                    var error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "Integration", ex.Message);
                    this.LogError("LMS.Convert", error);
                    throw new FaultException<Error>(error, ex.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        private FileDownloadDTO BuildUserGuide(LmsProviderEnum lms)
        {
            LmsProvider provider = LmsProviderModel.GetById((int)lms);
            var result = new FileDownloadDTO();

            result.downloadUrl = new Uri(new Uri((string)Settings.PortalUrl, UriKind.Absolute), $"content/lti-instructions/{provider.LmsProviderName}.pdf").ToString();

            result.fileName = string.Format("{0}.pdf", provider.LmsProviderName);
            result.title = "User Guide";

            string path = HttpContext.Current.Server.MapPath($"~/../Content/lti-instructions/{provider.LmsProviderName}.pdf");
            var file = new FileInfo(path);
            result.lastModifyDate = file.CreationTimeUtc;
            result.sizeInBytes = file.Length;

            return result;
        }

        private FileDownloadDTO BuildMobileDownload()
        {
            var result = new FileDownloadDTO();

            // TRICK: dup with FileController
            const string PublicFolderPath = "~/../Content/swf/pub";
            var filePattern = (string)Settings.MobileBuildSelector;
            var path = HostingEnvironment.MapPath(PublicFolderPath);
            var version = VersionProcessor.ProcessVersion(path, filePattern);
            var publicBuild = filePattern.Replace("*", version.ToString());
            result.downloadUrl = string.Format("{0}file/get-mobile-build", (string)this.Settings.PortalUrl);
            result.fileName = publicBuild;
            result.title = "Desktop + Mobile (Requires Connect 9.4.2+)";

            string physicalPath = Path.Combine(HttpContext.Current.Server.MapPath(PublicFolderPath), publicBuild);
            var file = new FileInfo(physicalPath);
            result.lastModifyDate = file.CreationTimeUtc;
            result.sizeInBytes = file.Length;

            return result;
        }

        private FileDownloadDTO BuildOfficeHoursePod()
        {
            var result = new FileDownloadDTO();

            Uri portalUrl = new Uri((string)Settings.PortalUrl, UriKind.Absolute);
            result.downloadUrl = new Uri(portalUrl, "content/lti-files/OfficeHoursPod.zip").ToString();

            result.fileName = "OfficeHoursPod.zip";
            result.title = "Office Hours Pod";

            string path = HttpContext.Current.Server.MapPath("~/../Content/lti-files/OfficeHoursPod.zip");
            var file = new FileInfo(path);
            result.lastModifyDate = file.CreationTimeUtc;
            result.sizeInBytes = file.Length;

            return result;
        }

        private FileDownloadDTO BuildBlackboardJar()
        {
            var result = new FileDownloadDTO();
            // 1.0.25
            string version = (string)this.Settings.BlackBoardJarVersion;
            Uri portalUrl = new Uri((string)Settings.PortalUrl, UriKind.Absolute);
            result.downloadUrl = new Uri(portalUrl, $"content/lti-files/edugame-cloud-ws-{version}.jar").ToString();
            result.fileName = string.Format("edugame-cloud-ws-{0}.jar", version);
            result.title = "Blackboard EGC Web Service";

            string path = HttpContext.Current.Server.MapPath($"~/../Content/lti-files/edugame-cloud-ws-{version}.jar");
            var file = new FileInfo(path);
            result.lastModifyDate = file.CreationTimeUtc;
            result.sizeInBytes = file.Length;

            return result;
        }

        private FileDownloadDTO BuildMoodleZip()
        {
            var result = new FileDownloadDTO();
            // 1.0.25
            string version = (string)this.Settings.MoodleZipVersion;
            Uri portalUrl = new Uri((string)Settings.PortalUrl, UriKind.Absolute);
            result.downloadUrl = new Uri(portalUrl, $"content/lti-files/edugamecloud_{version}.zip").ToString();
            result.fileName = string.Format("edugamecloud_{0}.zip", version);
            result.title = "Moodle EGC Web Service";

            string path = HttpContext.Current.Server.MapPath($"~/../Content/lti-files/edugamecloud_{version}.zip");
            var file = new FileInfo(path);
            result.lastModifyDate = file.CreationTimeUtc;
            result.sizeInBytes = file.Length;

            return result;
        }

        #endregion

    }

}