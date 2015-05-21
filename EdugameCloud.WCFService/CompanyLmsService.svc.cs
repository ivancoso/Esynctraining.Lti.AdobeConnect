// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using Castle.Core.Logging;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using FluentValidation.Results;

    /// <summary>
    /// The company LMS service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    public class CompanyLmsService : BaseService, ICompanyLmsService
    {
        #region Properties

        /// <summary>
        ///     Gets the company license model.
        /// </summary>
        private LmsCompanyModel LmsCompanyModel
        {
            get
            {
                return IoC.Resolve<LmsCompanyModel>();
            }
        }

        /// <summary>
        ///     Gets the DLAP API.
        /// </summary>
        private IBrainHoneyApi DlapAPI
        {
            get
            {
                return IoC.Resolve<IBrainHoneyApi>();
            }
        }

        /// <summary>
        ///     Gets the Moodle API.
        /// </summary>
        private IMoodleApi MoodleAPI
        {
            get
            {
                return IoC.Resolve<IMoodleApi>();
            }
        }

        /// <summary>
        ///     Gets the SOAP API.
        /// </summary>
        private IBlackBoardApi SoapAPI
        {
            get
            {
                return IoC.Resolve<IBlackBoardApi>();
            }
        }

        /// <summary>
        ///     Gets the SOAP API.
        /// </summary>
        private MeetingSetup MeetingSetup
        {
            get
            {
                return IoC.Resolve<MeetingSetup>();
            }
        }

        /// <summary>
        ///     Gets the LMS User model.
        /// </summary>
        private LmsUserModel LmsUserModel
        {
            get
            {
                return IoC.Resolve<LmsUserModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
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
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLmsDTO"/>.
        /// </returns>
        public CompanyLmsDTO Save(CompanyLmsDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                bool isTransient = resultDto.id == 0;
                LmsCompany entity = isTransient ? null : this.LmsCompanyModel.GetOneById(resultDto.id).Value;
                entity = ConvertDto(resultDto, entity);
                if (isTransient)
                {
                    entity.ConsumerKey = Guid.NewGuid().ToString();
                    entity.SharedSecret = Guid.NewGuid().ToString();
                }

                this.LmsCompanyModel.RegisterSave(entity);
                if (entity.LmsProvider.Id != (int)LmsProviderEnum.Canvas && !resultDto.enableProxyToolMode)
                {
                    var lmsUser = entity.AdminUser ?? new LmsUser { LmsCompany = entity, UserId = "0" };

                    lmsUser.Username = resultDto.lmsAdmin;
                    if (!string.IsNullOrEmpty(resultDto.lmsAdminPassword))
                    {
                        lmsUser.Password = resultDto.lmsAdminPassword;
                    }

                    lmsUser.Token = resultDto.lmsAdminToken;

                    LmsUserModel.RegisterSave(lmsUser, true);
                    entity.AdminUser = lmsUser;
                    LmsCompanyModel.RegisterSave(entity);
                }

                this.UpdateAdobeConnectFolder(isTransient, entity);

                return new CompanyLmsDTO(entity);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("CompanyLMS.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <returns>
        /// The <see cref="ConnectionInfoDTO"/>.
        /// </returns>
        public ConnectionInfoDTO TestConnection(ConnectionTestDTO test)
        {
            bool success = false;
            string info = string.Empty;

            if (!test.domain.StartsWithProtocol())
            {
                info = "Domain url should start with http:// or https://";
            }
            else
            {
                switch (test.type.ToLowerInvariant())
                {
                    case "ac":
                        success = this.TestACConnection(test, out info);
                        break;
                    case "brainhoney":
                        success = this.TestBrainHoneyConnection(test, out info);
                        break;
                    case "blackboard":
                        success = this.TestBlackBoardConnection(test, out info);
                        break;
                    case "moodle":
                        success = this.TestMoodleConnection(test, out info);
                        break;
                    case "sakai":
                        success = this.TestSakaiConnection(test, out info);
                        break;
                }    
            }
            
            return new ConnectionInfoDTO { status = success ? "Connected successfully" : "Failed to connect", info = info };
        }

        /// <summary>
        /// The update adobe connect folder.
        /// </summary>
        /// <param name="isTransient">
        /// The is transient.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        private void UpdateAdobeConnectFolder(bool isTransient, LmsCompany instance)
        {
            if (!isTransient && instance.UseUserFolder.GetValueOrDefault() == false)
            {
                AdobeConnectProvider acp = null;
                try
                {
                    acp = this.MeetingSetup.GetProvider(instance);
                }
                catch (Exception ex)
                {
                    IoC.Resolve<ILogger>().Error("UpdateAdobeConnectFolder", ex);
                }

                if (acp != null)
                {
                    var scoId = instance.ACScoId;
                    var resultedId = this.MeetingSetup.GetMeetingFolder(instance, acp, null);
                    if (scoId != resultedId)
                    {
                        instance.ACScoId = resultedId;
                        this.LmsCompanyModel.RegisterSave(instance);
                    }
                }
            }
        }

        /// <summary>
        /// The test MOODLE connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TestMoodleConnection(ConnectionTestDTO test, out string info)
        {
            return this.MoodleAPI.LoginAndCheckSession(out info, test.domain.IsSSL(), test.domain.RemoveHttpProtocolAndTrailingSlash(), test.login, test.password);
        }

        /// <summary>
        /// The test SAKAI connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        // ReSharper disable once UnusedParameter.Local
        private bool TestSakaiConnection(ConnectionTestDTO test, out string info)
        {
            info = "Not yet implemented";
            return false;
        }

        /// <summary>
        /// The test brain honey connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TestBrainHoneyConnection(ConnectionTestDTO test, out string info)
        {
            return this.DlapAPI.LoginAndCheckSession(out info, test.domain.RemoveHttpProtocolAndTrailingSlash(), test.login, test.password);
        }

        /// <summary>
        /// The test brain honey connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TestBlackBoardConnection(ConnectionTestDTO test, out string info)
        {
            var session = test.enableProxyToolMode

                              ? this.SoapAPI.LoginToolAndCreateAClient(
                                  out info,
                                  test.domain.IsSSL(),
                                  test.domain,
                                  test.password)

                              : this.SoapAPI.LoginUserAndCreateAClient(
                                  out info,
                                  test.domain.IsSSL(),
                                  test.domain,
                                  test.login,
                                  test.password);
            return session != null;
        }

        /// <summary>
        /// The test AC connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TestACConnection(ConnectionTestDTO test, out string info)
        {
            info = string.Empty;
            var domainUrl = test.domain.ToLowerInvariant();
            if (!domainUrl.StartsWithProtocol())
            {
                info = "Adobe Connect Domain url should start with http or https";
                return false;
            }

            var fixedUrl = domainUrl.EndsWith("/") ? domainUrl : string.Format("{0}/", domainUrl);
            fixedUrl = fixedUrl.EndsWith("api/xml/") ? fixedUrl : string.Format("{0}api/xml", fixedUrl);

            var provider = new AdobeConnectProvider(new ConnectionDetails { ServiceUrl = fixedUrl });

            var result = provider.Login(new UserCredentials(test.login, test.password));

            if (!result.Success)
            {
                var error = FormatErrorFromAC(result).With(x => x.error);
                if (error != null)
                {
                    info = error.errorMessage;
                }
            }

            return result.Success;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="LmsCompany"/>.
        /// </returns>
        private LmsCompany ConvertDto(CompanyLmsDTO dto, LmsCompany instance)
        {
            instance = instance ?? new LmsCompany();
            if (!string.IsNullOrEmpty(dto.acPassword))
            {
                instance.AcPassword = dto.acPassword;
            }

            instance.AcServer = dto.acServer;
            instance.AcUsername = dto.acUsername;
            instance.CompanyId = dto.companyId;
            instance.ConsumerKey = dto.consumerKey;
            instance.CreatedBy = dto.createdBy;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            instance.LmsProvider = this.LmsProviderModel.GetOneByName(dto.lmsProvider).Value;
            instance.ModifiedBy = this.UserModel.GetOneById(dto.modifiedBy).Value.Return(x => x.Id, dto.createdBy);
            instance.SharedSecret = dto.sharedSecret;
            instance.PrimaryColor = dto.primaryColor;
            instance.Title = dto.title;
            instance.UseUserFolder = dto.useUserFolder;
            instance.CanRemoveMeeting = dto.canRemoveMeeting;
            instance.CanEditMeeting = dto.canEditMeeting;
            instance.IsSettingsVisible = dto.isSettingsVisible;
            instance.EnableOfficeHours = dto.enableOfficeHours;
            instance.EnableStudyGroups = dto.enableStudyGroups;
            instance.EnableCourseMeetings = dto.enableCourseMeetings;
            instance.ShowEGCHelp = dto.showEGCHelp;
            instance.ShowLmsHelp = dto.showLmsHelp;
            instance.AddPrefixToMeetingName = dto.addPrefixToMeetingName;
            instance.UserFolderName = dto.userFolderName;
            instance.EnableProxyToolMode = dto.enableProxyToolMode;
            instance.ProxyToolSharedPassword = dto.proxyToolPassword;
            if (!string.IsNullOrWhiteSpace(dto.lmsDomain))
            {
                instance.LmsDomain = dto.lmsDomain.RemoveHttpProtocolAndTrailingSlash();
                instance.UseSSL = dto.lmsDomain.IsSSL();
            }

            instance.DenyACUserCreation = !dto.allowUserCreation;
            instance.LoginUsingCookie = !dto.showAuthToken;
            instance.ACUsesEmailAsLogin = dto.acUsesEmailAsLogin;
            instance.ShowAnnouncements = dto.enableAnnouncements;
            LmsCompanyModel.UpdateCompanySetting(instance, "UseSynchronizedUsers", dto.useSynchronizedUsers.ToString());
            return instance;
        }

        #endregion

    }

}