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

        private TestConnectionService TestConnectionService
        {
            get
            {
                return IoC.Resolve<TestConnectionService>();
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

            bool isTransient = resultDto.id == 0;
            LmsCompany entity = isTransient ? null : this.LmsCompanyModel.GetOneById(resultDto.id).Value;

            string lmsPassword = resultDto.lmsAdminPassword;
            if (!isTransient && string.IsNullOrWhiteSpace(resultDto.lmsAdminPassword))
            {
                if ((entity.LmsProvider.Id == (int)LmsProviderEnum.Moodle)
                || ((entity.LmsProvider.Id == (int)LmsProviderEnum.Blackboard) && !resultDto.enableProxyToolMode))
                {
                    lmsPassword = entity.AdminUser.Password;
                }
                else if ((entity.LmsProvider.Id == (int)LmsProviderEnum.Blackboard) && resultDto.enableProxyToolMode)
                {
                    lmsPassword = resultDto.proxyToolPassword;
                }
            }

            ConnectionInfoDTO lmsConnectionTest = TestConnection(new ConnectionTestDTO
            {
                domain = resultDto.lmsDomain,
                enableProxyToolMode = resultDto.enableProxyToolMode,
                login = resultDto.lmsAdmin,
                password = lmsPassword,
                type = resultDto.lmsProvider,
            });

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
                        this.LmsProviderModel.GetOneByName(resultDto.lmsProvider).Value.LmsProviderName,
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
            return TestConnectionService.TestConnection(test);
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

            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.LabelMeeting, dto.labelMeeting);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.LabelOfficeHour, dto.labelOfficeHour);
            UpdateOrDeleteSetting(instance, LmsCompanySettingNames.LabelStudyGroup, dto.labelStudyGroup);

            LmsCompanyModel.UpdateCompanySetting(instance, LmsCompanySettingNames.EnableMeetingReuse, dto.enableMeetingReuse.ToString());
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

        #endregion

    }

}