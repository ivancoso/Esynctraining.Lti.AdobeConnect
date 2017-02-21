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
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.DTO;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.DTO;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using FluentValidation.Results;
    using Newtonsoft.Json;

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

        private LmsCompanyModel LmsCompanyModel => IoC.Resolve<LmsCompanyModel>();

        private LmsCompanySettingModel LmsCompanySettingModel => IoC.Resolve<LmsCompanySettingModel>();

        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();
        
        private LmsUserModel LmsUserModel => IoC.Resolve<LmsUserModel>();
        
        private LmsProviderModel LmsProviderModel => IoC.Resolve<LmsProviderModel>();

        private TestConnectionService TestConnectionService => IoC.Resolve<TestConnectionService>();

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

            bool isTransient = resultDto.id == 0;
            LmsCompany entity = isTransient ? null : this.LmsCompanyModel.GetOneById(resultDto.id).Value;

            string lmsPassword = resultDto.lmsAdminPassword;
            if (!isTransient && string.IsNullOrWhiteSpace(resultDto.lmsAdminPassword))
            {
                if ((entity.LmsProviderId == (int)LmsProviderEnum.Moodle && string.IsNullOrWhiteSpace(entity.GetSetting<string>(LmsCompanySettingNames.MoodleCoreServiceToken)))
                || ((entity.LmsProviderId == (int)LmsProviderEnum.Blackboard) && !resultDto.enableProxyToolMode))
                {
                    lmsPassword = entity.AdminUser.Password;
                }
            }

            if ((this.LmsProviderModel.GetByName(resultDto.lmsProvider).Id == (int)LmsProviderEnum.Blackboard) && resultDto.enableProxyToolMode)
            {
                lmsPassword = resultDto.proxyToolPassword;
            }

            ConnectionInfoDTO lmsConnectionTest;
            if (!string.IsNullOrWhiteSpace(resultDto.moodleCoreServiceToken))
            {
                lmsConnectionTest = new ConnectionInfoDTO { status = OkMessage, info = "Test connection is not supported for Moodle Token mode" };
            }
            else
            {
                lmsConnectionTest = TestConnection(new ConnectionTestDTO
                {
                    domain = resultDto.lmsDomain,
                    enableProxyToolMode = resultDto.enableProxyToolMode,
                    login = resultDto.lmsAdmin,
                    password = lmsPassword,
                    type = resultDto.lmsProvider,
                });
            }

            string acPassword = (isTransient || !string.IsNullOrWhiteSpace(resultDto.acPassword)) 
                ? resultDto.acPassword 
                : entity.AcPassword;

            string acConnectionInfo;
            bool loginSameAsEmail;
            bool acConnectionTest = TestConnectionService.TestACConnection(new ConnectionTestDTO
            {
                domain = resultDto.acServer,
                enableProxyToolMode = resultDto.enableProxyToolMode,
                login = resultDto.acUsername,
                password = acPassword,
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
                        this.LmsProviderModel.GetByName(resultDto.lmsProvider).LmsProviderName,
                        lmsConnectionTest.info);
                }

                if (!acConnectionTest)
                {
                    message.AppendFormat("Adobe Connect connection failed. ({0})",
                        acConnectionInfo);
                }

                licenseTestResultMessage = message.ToString();
            }

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

            LmsProvider lmsProvider = LmsProviderModel.GetById(entity.LmsProviderId);
            return new CompanyLmsOperationDTO
            {
                companyLmsVO = new CompanyLmsDTO(entity, lmsProvider, Settings),
                message = licenseTestResultMessage,
            };
        }

        public ConnectionInfoDTO TestConnection(ConnectionTestDTO test)
        {
            return TestConnectionService.TestConnection(test);
        }
        
        public int DeleteById(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            LmsCompanyModel.DeleteWithDependencies(id);
            
            return id;
        }

        #endregion

        #region Methods

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
            instance.EnableMultipleMeetings = dto.enableMultipleMeetings;
            instance.AcUsername = dto.acUsername;
            instance.CompanyId = dto.companyId;
            instance.ConsumerKey = dto.consumerKey;
            instance.CreatedBy = dto.createdBy;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            var lmsProvider = this.LmsProviderModel.GetByName(dto.lmsProvider);
            instance.LmsProviderId = lmsProvider.Id;
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

            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.LabelMeeting, dto.labelMeeting);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.LabelOfficeHour, dto.labelOfficeHour);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.LabelStudyGroup, dto.labelStudyGroup);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.VirtualClassroomsLabel, dto.labelVirtualClassroom);

            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.ShowMeetingSummary, dto.showSummary);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.ShowMeetingTime, dto.showTime);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.ShowMeetingDuration, dto.showDuration);

            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.CanRemoveRecordings, dto.canRemoveRecordings);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.AutoPublishRecordings, dto.autoPublishRecordings);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.ForcedAddInInstallation, dto.forcedAddInInstallation);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.LanguageId, dto.languageId);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Mp4ServiceLicenseKey, dto.mp4ServiceLicenseKey);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey, dto.mp4ServiceWithSubtitlesLicenseKey);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.SeminarsLabel, dto.labelSeminar);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.UseSakaiEvents, dto.useSakaiEvents);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.EnableMeetingSessions, dto.enableMeetingSessions);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.EnableVirtualClassrooms, dto.enableVirtualClassrooms);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.EnableMyContent, dto.enableMyContent);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.EnableAddGuest, dto.enableAddGuest);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.EnableSetUserRole, dto.enableSetUserRole);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.EnableRemoveUser, dto.enableRemoveUser);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.CanStudentCreateStudyGroup, dto.canStudentCreateStudyGroup);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.MoodleCoreServiceToken, dto.moodleCoreServiceToken);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.MoodleQuizServiceToken, dto.moodleQuizServiceToken);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.IsPdfMeetingUrl, dto.isPdfMeetingUrl);

            LmsCompanyModel.UpdateCompanySetting(instance, LmsCompanySettingNames.EnableMeetingReuse, dto.enableMeetingReuse.ToString());
            LmsCompanyModel.UpdateCompanySetting(instance, LmsCompanySettingNames.UseSynchronizedUsers, dto.useSynchronizedUsers.ToString());
            LmsCompanyModel.UpdateCompanySetting(instance, LmsCompanySettingNames.SeminarsEnable, dto.enableSeminars.ToString());
            LmsCompanyModel.UpdateCompanySetting(instance, LmsCompanySettingNames.EnableAuditGuestEntry, dto.enableAuditGuestEntry.ToString());

            LmsCompanyModel.UpdateCompanySetting(instance, LmsCompanySettingNames.ShowAudioProfile, dto.showAudioProfile.ToString());
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.AudioProfileUnique, dto.audioProfileUnique.ToString());

            //OAuth options
            if (lmsProvider.Id == (int) LmsProviderEnum.Desire2Learn || lmsProvider.Id == (int)LmsProviderEnum.Canvas)
            {
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.IsOAuthSandbox, dto.isSandbox);
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.OAuthAppId, dto.oAuthAppId);
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.OAuthAppKey, dto.oAuthAppKey);
            }
            else if (lmsProvider.Id == (int)LmsProviderEnum.Blackboard)
            {
                if (dto.enableProxyToolMode)
                {
                    string json = (dto.additionalLmsDomains == null) || (dto.additionalLmsDomains.Length == 0)
                        ? null
                        : JsonConvert.SerializeObject(dto.additionalLmsDomains);
                    UpdateOrDeleteSetting(instance, LmsCompanySettingNames.AdditionalLmsDomains, json);
                }
                else
                {
                    UpdateOrDeleteSetting(instance, LmsCompanySettingNames.AdditionalLmsDomains, null);
                }
            }

            ProcessTelephony(dto, instance);
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

        private void ProcessTelephony(CompanyLmsDTO dto, LmsCompany instance)
        {
            if (dto.Telephony != null)
            {
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Telephony.ActiveProfile,
                    dto.Telephony.ActiveProfile);
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Telephony.CourseMeetingOption,
                    dto.Telephony.CourseMeetingOption);
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Telephony.OfficeHoursOption,
                    dto.Telephony.OfficeHoursOption);
                UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Telephony.StudyGroupOption,
                    dto.Telephony.StudyGroupOption);
                if (dto.Telephony.MeetingOne != null)
                {
                    UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Telephony.MeetingOne.OwningAccountNumber,
                        dto.Telephony.MeetingOne.OwningAccountNumber);
                    UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Telephony.MeetingOne.UserName,
                        dto.Telephony.MeetingOne.UserName);
                    UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Telephony.MeetingOne.SecretHashKey,
                        dto.Telephony.MeetingOne.SecretHashKey);
                }
                if (dto.Telephony.Arkadin != null)
                {
                    UpdateOrDeleteSetting(instance, LmsCompanySettingNames.Telephony.Arkadin.UserName,
                        dto.Telephony.Arkadin.UserName);
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

        #endregion

    }

}