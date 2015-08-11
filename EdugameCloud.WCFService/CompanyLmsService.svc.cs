using EdugameCloud.Lti.Core.Constants;
// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Text;
    using BbWsClient;
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
    using EdugameCloud.WCFService.DTO;
    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
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
        private const string OkMessage = "Connected successfully";

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

        private LmsCompanySettingModel LmsCompanySettingModel
        {
            get
            {
                return IoC.Resolve<LmsCompanySettingModel>();
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
        public CompanyLmsOperationDTO Save(CompanyLmsDTO resultDto)
        {
            ValidationResult validationResult;
            if (!this.IsValid(resultDto, out validationResult))
            {
                var error = this.GenerateValidationError(validationResult);
                this.LogError("CompanyLMS.Save", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            ConnectionInfoDTO lmsConnectionTest = TestConnection(new ConnectionTestDTO
            {
                domain = resultDto.lmsDomain,
                enableProxyToolMode = resultDto.enableProxyToolMode,
                login = resultDto.lmsAdmin,
                password = resultDto.lmsAdminPassword,
                type = resultDto.lmsProvider,
            });

            string acConnectionInfo;
            bool loginSameAsEmail;
            bool acConnectionTest = TestACConnection(new ConnectionTestDTO
            {
                domain = resultDto.acServer,
                enableProxyToolMode = resultDto.enableProxyToolMode,
                login = resultDto.acUsername,
                password = resultDto.acPassword,
                type = "ac",
            }, out acConnectionInfo, out loginSameAsEmail);

            string licenseTestResultMessage = null;
            if (lmsConnectionTest.status != OkMessage
                || !acConnectionTest)
            {
                var message = new StringBuilder("LMS License is inactive due to following reasons: \r\n");
                if (lmsConnectionTest.status != OkMessage)
                {
                    message.AppendFormat("{0} connection failed. ({1}) \r\n",
                        this.LmsProviderModel.GetOneByName(resultDto.lmsProvider).Value.LmsProviderName,
                        lmsConnectionTest.info);
                }

                if (!acConnectionTest)
                {
                    message.AppendFormat("Adobe Connect connection failed. ({0})",
                        acConnectionInfo);
                }

                //var error = new Error(
                //    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                //    "Connection failed",
                //    message.ToString());
                //throw new FaultException<Error>(error, message.ToString());
                licenseTestResultMessage = message.ToString();
            }

            bool isTransient = resultDto.id == 0;
            LmsCompany entity = isTransient ? null : this.LmsCompanyModel.GetOneById(resultDto.id).Value;
            entity = ConvertDto(resultDto, entity);

            // NOTE: always use setting from AC not UI
            entity.ACUsesEmailAsLogin = loginSameAsEmail;
            entity.IsActive = lmsConnectionTest.status == OkMessage && acConnectionTest;

            if (isTransient)
            {
                entity.ConsumerKey = Guid.NewGuid().ToString();
                entity.SharedSecret = Guid.NewGuid().ToString();
            }

            this.LmsCompanyModel.RegisterSave(entity);
            this.LmsCompanyModel.ProcessLmsAdmin(entity, resultDto, LmsUserModel, LmsCompanyModel);

            this.UpdateAdobeConnectFolder(isTransient, entity);

            return new CompanyLmsOperationDTO 
            { 
                companyLmsVO = new CompanyLmsDTO(entity),
                message = licenseTestResultMessage,
            };
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
                        bool loginSameAsEmail;
                        success = this.TestACConnection(test, out info, out loginSameAsEmail);
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

            return new ConnectionInfoDTO { status = success ? OkMessage : "Failed to connect", info = info };
        }

        #endregion

        #region Methods

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
                IAdobeConnectProxy acp = null;
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

        private bool TestBlackBoardConnection(ConnectionTestDTO test, out string info)
        {
            WebserviceWrapper session = test.enableProxyToolMode
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

            bool success = session != null;
            if (session != null)
                session.logout();

            return success;
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
        private bool TestACConnection(ConnectionTestDTO test, out string info, out bool loginSameAsEmail)
        {
            loginSameAsEmail = false;
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

                return false;
            }

            StatusInfo status;
            UserInfo usr = provider.GetUserInfo(out status);

            if (status.Code != StatusCodes.ok)
            {
                logger.ErrorFormat("GetPasswordPolicies.GetUserInfo. AC error. {0}.", status.GetErrorInfo());
                info = status.GetErrorInfo();
                return false;
            }

            if ((usr != null) && usr.AccountId.HasValue)
            {
                FieldCollectionResult fields = provider.GetAclFields(usr.AccountId.Value);

                if (!fields.Success)
                {
                    logger.ErrorFormat("GetPasswordPolicies.GetAclFields. AC error. {0}.", fields.Status.GetErrorInfo());
                    info = fields.Status.GetErrorInfo();
                    return false;
                }

                loginSameAsEmail = "YES".Equals(GetField(fields, "login-same-as-email"), StringComparison.OrdinalIgnoreCase);
                return true;
            }

            logger.Error("GetPasswordPolicies. Account is NULL. Check Adobe Connect account permissions. Admin account expected.");
            info = "Check Adobe Connect account permissions. Admin account expected.";
            return false;            
        }

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
            instance.UseFLV = dto.useFLV;
            instance.UseMP4 = dto.useMP4;
            instance.AcUsername = dto.acUsername;
            instance.CompanyId = dto.companyId;
            instance.ConsumerKey = dto.consumerKey;
            instance.CreatedBy = dto.createdBy;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            var lmsProvider = this.LmsProviderModel.GetOneByName(dto.lmsProvider).Value;
            instance.LmsProvider = lmsProvider;
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

            // TRICK: always should set directly
            instance.IsActive = false;// dto.isActive;
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
            instance.MeetingNameFormatterId = dto.meetingNameFormatterId;

            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.SupportPageHtml, dto.supportPageHtml);

            LmsCompanyModel.UpdateCompanySetting(instance, LmsCompanySettingNames.UseSynchronizedUsers, dto.useSynchronizedUsers.ToString());
            //D2L only options
            if (lmsProvider.Id == (int) LmsProviderEnum.Desire2Learn)
            {
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.IsD2LSandbox, dto.isSandbox);
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.D2LAppId, dto.d2lAppId);
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.D2LAppKey, dto.d2lAppKey);
            }

            ProcessRoleMapping(dto, instance);

            return instance;
        }

        private void UpdateOrDeleteSetting(LmsCompany lmsCompany, string settingName, object settingValue)
        {
            var stringSettingValue = settingValue as string;
            if (settingValue != null && (stringSettingValue == null || !string.IsNullOrEmpty(stringSettingValue)))
            {
                LmsCompanyModel.UpdateCompanySetting(lmsCompany, settingName,
                    settingValue.ToString());
            }
            else
            {
                // todo: handle not-nullable values correctly if necessary
                var setting = LmsCompanySettingModel.GetOneByLmsCompanyIdAndSettingName(lmsCompany.Id, settingName);
                if (setting.Value != null)
                {
                    lmsCompany.Settings.Remove(lmsCompany.Settings.FirstOrDefault(x => x.Id == setting.Value.Id));
                    LmsCompanySettingModel.RegisterDelete(setting.Value, true);
                }
            }
        }

        private static void ProcessRoleMapping(CompanyLmsDTO dto, LmsCompany instance)
        {
            if (dto.roleMapping == null)
                dto.roleMapping = new LmsCompanyRoleMappingDTO[0];

            var oldMapsToDelete = new List<LmsCompanyRoleMapping>();
            foreach (LmsCompanyRoleMapping old in instance.RoleMappings)
            {
                var newSetRole = dto.roleMapping.FirstOrDefault(x => x.lmsRoleName == old.LmsRoleName);
                if (newSetRole == null)
                {
                    oldMapsToDelete.Add(old);
                }
                else
                {
                    old.AcRole = newSetRole.acRole;
                }
            }
            if (oldMapsToDelete.Count > 0)
                foreach (var item in oldMapsToDelete)
                    instance.RoleMappings.Remove(item);

            foreach (LmsCompanyRoleMappingDTO newRole in dto.roleMapping)
            {
                if (!instance.RoleMappings.Any(map => map.LmsRoleName == newRole.lmsRoleName))
                {
                    var map = new LmsCompanyRoleMapping
                    {
                        LmsCompany = instance,
                        LmsRoleName = newRole.lmsRoleName,
                        AcRole = newRole.acRole,
                        IsDefaultLmsRole = newRole.isDefaultLmsRole,
                    };
                    instance.RoleMappings.Add(map);
                }
            }
        }

        private static string GetField(FieldCollectionResult value, string fieldName)
        {
            Field field = value.Values.FirstOrDefault(x => x.FieldId == fieldName);
            if (field == null)
            {
                return null;
            }

            return field.Value;
        }

        #endregion

    }

}