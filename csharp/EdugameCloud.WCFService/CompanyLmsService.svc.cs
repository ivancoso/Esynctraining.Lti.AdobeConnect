using EdugameCloud.Lti.Core.Constants;
using Esynctraining.Lti.Lms.Common.Constants;

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
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.DTO;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.DTO;
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
            if (!isTransient
                && string.IsNullOrWhiteSpace(resultDto.lmsAdminPassword)
                && ((entity.LmsProviderId == (int) LmsProviderEnum.Moodle &&
                     string.IsNullOrWhiteSpace(entity.GetSetting<string>(LmsLicenseSettingNames.MoodleCoreServiceToken)))
                    || (entity.LmsProviderId == (int) LmsProviderEnum.Blackboard && !resultDto.enableProxyToolMode)
                    || entity.LmsProviderId == (int) LmsProviderEnum.AgilixBuzz)
            )
            {
                lmsPassword = entity.AdminUser.Password;
            }

            if ((this.LmsProviderModel.GetByShortName(resultDto.lmsProvider).Id == (int)LmsProviderEnum.Blackboard) && resultDto.enableProxyToolMode)
            {
                lmsPassword = resultDto.proxyToolPassword;
            }

            if ((this.LmsProviderModel.GetByShortName(resultDto.lmsProvider).Id == (int)LmsProviderEnum.Schoology))
            {
                // TRICK: for test-connection only
                resultDto.lmsAdmin = resultDto.schoologyConsumerKey;
                lmsPassword = resultDto.schoologyConsumerSecret;
            }

            ConnectionInfoDTO lmsConnectionTest;
            if (!string.IsNullOrWhiteSpace(resultDto.moodleCoreServiceToken))
            {
                lmsConnectionTest = new ConnectionInfoDTO { status = OkMessage, info = "Test connection is not supported for Moodle Token mode" };
            }
            else
            {
                if (this.LmsProviderModel.GetByShortName(resultDto.lmsProvider).Id == (int)LmsProviderEnum.Haiku)
                {
                    lmsConnectionTest = TestConnection(new ConnectionTestDTO
                    {
                        domain = resultDto.lmsDomain,
                        enableProxyToolMode = resultDto.enableProxyToolMode,
                        login = resultDto.lmsAdmin,
                        password = lmsPassword,
                        type = resultDto.lmsProvider,
                        consumerKey = resultDto.haikuConsumerKey,
                        consumerSecret = resultDto.haikuConsumerSecret,
                        token = resultDto.haikuToken,
                        tokenSecret = resultDto.haikuTokenSecret
                    });
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
                        this.LmsProviderModel.GetByShortName(resultDto.lmsProvider).LmsProviderName,
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
            var lmsProvider = this.LmsProviderModel.GetByShortName(dto.lmsProvider);
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

            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.SupportPageHtml, dto.supportPageHtml);

            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.LabelMeeting, dto.labelMeeting);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.LabelOfficeHour, dto.labelOfficeHour);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.LabelStudyGroup, dto.labelStudyGroup);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.VirtualClassroomsLabel, dto.labelVirtualClassroom);

            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.ShowMeetingSummary, dto.showSummary);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.ShowMeetingTime, dto.showTime);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.ShowMeetingDuration, dto.showDuration);

            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.CanRemoveRecordings, dto.canRemoveRecordings);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.AutoPublishRecordings, dto.autoPublishRecordings);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.ForcedAddInInstallation, dto.forcedAddInInstallation);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.LanguageId, dto.languageId);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Mp4ServiceLicenseKey, dto.mp4ServiceLicenseKey);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Mp4ServiceWithSubtitlesLicenseKey, dto.mp4ServiceWithSubtitlesLicenseKey);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.SeminarsLabel, dto.labelSeminar);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.UseSakaiEvents, dto.useSakaiEvents);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.EnableMeetingSessions, dto.enableMeetingSessions);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.EnableCanvasExportToCalendar, dto.EnableCanvasExportToCalendar);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.EnableMyContent, dto.enableMyContent);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.EnableAddGuest, dto.enableAddGuest);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.EnableSetUserRole, dto.enableSetUserRole);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.EnableRemoveUser, dto.enableRemoveUser);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.CanStudentCreateStudyGroup, dto.canStudentCreateStudyGroup);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.EnableVirtualClassrooms, dto.enableVirtualClassrooms);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.NamedVirtualClassroomManager, dto.namedVirtualClassroomManager);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.HidePrivateRecordingsForStudents, dto.HidePrivateRecordingsForStudents);

            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.MoodleCoreServiceToken, dto.moodleCoreServiceToken);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.MoodleQuizServiceToken, dto.moodleQuizServiceToken);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.SchoologyConsumerKey, dto.schoologyConsumerKey);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.SchoologyConsumerSecret, dto.schoologyConsumerSecret);

            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.HaikuConsumerKey, dto.haikuConsumerKey);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.HaikuConsumerSecret, dto.haikuConsumerSecret);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.HaikuToken, dto.haikuToken);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.HaikuTokenSecret, dto.haikuTokenSecret);

            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.BridgeApiTokenKey, dto.bridgeApiTokenKey);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.BridgeApiTokenSecret, dto.bridgeApiTokenSecret);

            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.IsPdfMeetingUrl, dto.isPdfMeetingUrl);

            LmsCompanyModel.UpdateCompanySetting(instance, LmsLicenseSettingNames.EnableMeetingReuse, dto.enableMeetingReuse.ToString());
            LmsCompanyModel.UpdateCompanySetting(instance, LmsLicenseSettingNames.UseSynchronizedUsers, dto.useSynchronizedUsers.ToString());
            LmsCompanyModel.UpdateCompanySetting(instance, LmsLicenseSettingNames.SeminarsEnable, dto.enableSeminars.ToString());
            LmsCompanyModel.UpdateCompanySetting(instance, LmsLicenseSettingNames.EnableAuditGuestEntry, dto.enableAuditGuestEntry.ToString());

            LmsCompanyModel.UpdateCompanySetting(instance, LmsLicenseSettingNames.ShowAudioProfile, dto.showAudioProfile.ToString());
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.AudioProfileUnique, dto.audioProfileUnique.ToString());
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.UseCourseSections, dto.UseCourseSections);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.UseCourseMeetingsCustomLayout, dto.UseCourseMeetingsCustomLayout);
            UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.EnableOfficeHoursSlots, dto.EnableOfficeHoursSlots);

            //OAuth options
            if (lmsProvider.Id == (int) LmsProviderEnum.Brightspace || lmsProvider.Id == (int)LmsProviderEnum.Canvas)
            {
                UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.IsOAuthSandbox, dto.isSandbox);
                UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.OAuthAppId, dto.oAuthAppId);
                UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.OAuthAppKey, dto.oAuthAppKey);
            }
            else if (lmsProvider.Id == (int)LmsProviderEnum.Blackboard)
            {
                if (dto.enableProxyToolMode)
                {
                    string json = (dto.additionalLmsDomains == null) || (dto.additionalLmsDomains.Length == 0)
                        ? null
                        : JsonConvert.SerializeObject(dto.additionalLmsDomains);
                    UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.AdditionalLmsDomains, json);
                }
                else
                {
                    UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.AdditionalLmsDomains, null);
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
                UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Telephony.ActiveProfile,
                    dto.Telephony.ActiveProfile);
                UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Telephony.CourseMeetingOption,
                    dto.Telephony.CourseMeetingOption);
                UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Telephony.OfficeHoursOption,
                    dto.Telephony.OfficeHoursOption);
                UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Telephony.StudyGroupOption,
                    dto.Telephony.StudyGroupOption);
                if (dto.Telephony.MeetingOne != null)
                {
                    UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Telephony.MeetingOne.OwningAccountNumber,
                        dto.Telephony.MeetingOne.OwningAccountNumber);
                    UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Telephony.MeetingOne.UserName,
                        dto.Telephony.MeetingOne.UserName);
                    UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Telephony.MeetingOne.SecretHashKey,
                        dto.Telephony.MeetingOne.SecretHashKey);
                }
                if (dto.Telephony.Arkadin != null)
                {
                    UpdateOrDeleteSetting(instance, LmsLicenseSettingNames.Telephony.Arkadin.UserName,
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
                    old.IsTeacherRole = newSetRole.isTeacherRole;
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
                        IsTeacherRole = newRole.isTeacherRole,
                    };
                    instance.RoleMappings.Add(map);
                }
            }
        }

        #endregion

    }

}