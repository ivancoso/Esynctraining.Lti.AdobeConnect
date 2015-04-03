// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.DTO;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    /// The company license service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    public class CompanyService : BaseService, Contracts.ICompanyService
    {
        #region Properties

        /// <summary>
        /// Gets the company model.
        /// </summary>
        private CompanyModel CompanyModel
        {
            get
            {
                return IoC.Resolve<CompanyModel>();
            }
        }

        /// <summary>
        /// Gets the company license model.
        /// </summary>
        private LmsCompanyModel LmsCompanyModel
        {
            get
            {
                return IoC.Resolve<LmsCompanyModel>();
            }
        }

        /// <summary>
        /// Gets the company license model.
        /// </summary>
        private LmsProviderModel LmsProviderModel
        {
            get
            {
                return IoC.Resolve<LmsProviderModel>();
            }
        }

        /// <summary>
        /// Gets the LMS user model.
        /// </summary>
        private LmsUserModel LmsUserModel
        {
            get
            {
                return IoC.Resolve<LmsUserModel>();
            }
        }

        /// <summary>
        /// Gets the company model.
        /// </summary>
        private CompanyThemeModel CompanyThemeModel
        {
            get
            {
                return IoC.Resolve<CompanyThemeModel>();
            }
        }

        /// <summary>
        /// Gets the company license model.
        /// </summary>
        private CompanyLicenseModel CompanyLicenseModel
        {
            get
            {
                return IoC.Resolve<CompanyLicenseModel>();
            }
        }

        /// <summary>
        /// Gets the time zone model.
        /// </summary>
        private TimeZoneModel TimeZoneModel
        {
            get
            {
                return IoC.Resolve<TimeZoneModel>();
            }
        }

        /// <summary>
        /// Gets the language model.
        /// </summary>
        private LanguageModel LanguageModel
        {
            get
            {
                return IoC.Resolve<LanguageModel>();
            }
        }

        /// <summary>
        ///     Gets the UserRole model.
        /// </summary>
        private UserRoleModel UserRoleModel
        {
            get
            {
                return IoC.Resolve<UserRoleModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteById(int id)
        {
            Company company;
            CompanyModel model = this.CompanyModel;
            if ((company = model.GetOneById(id).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("Company.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (company.PrimaryContact != null)
            {
                company.PrimaryContact = null;
                model.RegisterSave(company, true);
            }

            var userModel = this.UserModel;
            foreach (var user in company.Users)
            {
                userModel.RealDelete(user, true);
            }

            model.Refresh(ref company);
            model.RegisterDelete(company, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Delete, company.Id, company.Id);
            return id;
        }

        /// <summary>
        /// The request license upgrade.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        public void RequestLicenseUpgrade(int companyId)
        {
            Company company;
            if ((company = this.CompanyModel.GetOneById(companyId).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("Company.RequestLicenseUpgrade", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            this.SendLicenseUpgradeEmail(company);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="CompanyDTO"/>.
        /// </returns>
        public CompanyDTO[] GetAll()
        {
            return this.CompanyModel.GetAllWithRelated().Select(x => new CompanyDTO(x)).ToArray();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLicenseDTO"/>.
        /// </returns>
        public CompanyLicenseDTO[] GetLicenseHistoryByCompanyId(int companyId)
        {
            Company company;
            if ((company = this.CompanyModel.GetOneById(companyId).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("Company.RequestLicenseUpgrade", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return
                company.Licenses.ToList()
                    .OrderByDescending(x => x.ExpiryDate)
                    .Select(x => new CompanyLicenseDTO(x))
                    .ToArray();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyDTO"/>.
        /// </returns>
        public CompanyDTO GetById(int id)
        {
            Company company;
            if ((company = this.CompanyModel.GetOneById(id).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("Company.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new CompanyDTO(company);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyThemeDTO"/>.
        /// </returns>
        public CompanyThemeDTO GetThemeByCompanyId(int id)
        {
            Company company;
            if ((company = this.CompanyModel.GetOneById(id).Value) == null || (company.CurrentLicense ?? company.FutureActiveLicense).With(cl => cl.LicenseStatus != CompanyLicenseStatus.Enterprise))
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("Company.GetThemeByCompanyId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return company.Theme == null ? null : new CompanyThemeDTO(company.Id, company.Theme);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyThemeDTO"/>.
        /// </returns>
        public CompanyThemeDTO GetThemeById(string id)
        {
            Guid themeId;
            if (!Guid.TryParse(id, out themeId))
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError(string.Format("Company.GetThemeById. id={0}", id), error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            CompanyTheme companyTheme;
            Company company;
            if ((companyTheme = this.CompanyThemeModel.GetOneById(themeId).Value) == null 
                || ((company = this.CompanyModel.GetOneByCompanyThemeId(companyTheme.Id).Value) == null) 
                || (company.CurrentLicense ?? company.FutureActiveLicense).With(cl => cl.LicenseStatus != CompanyLicenseStatus.Enterprise))
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("Company.GetThemeById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new CompanyThemeDTO(company.Id, companyTheme);
        }

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="companyId">
        /// The user id.
        /// </param>
        public void ActivateById(int companyId)
        {
            var model = this.CompanyModel;
            Company company;
            if ((company = model.GetOneById(companyId).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("Company.ActivateById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            company.Status = CompanyStatus.Active;
            model.RegisterSave(company, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Update, company.Id, company.Id);
        }

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="companyId">
        /// The user id.
        /// </param>
        public void DeactivateById(int companyId)
        {
            var model = this.CompanyModel;
            Company company;
            if ((company = model.GetOneById(companyId).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("Company.DeactivateById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            company.Status = CompanyStatus.Inactive;
            model.RegisterSave(company, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Update, company.Id, company.Id);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="companyThemeDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyThemeDTO"/>.
        /// </returns>
        public CompanyThemeDTO SaveTheme(CompanyThemeDTO companyThemeDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(companyThemeDTO, out validationResult))
            {
                var company = this.CompanyModel.GetOneById(companyThemeDTO.companyId).Value;
                var companyTheme = company.With(x => x.Theme);
                companyTheme = this.Convert(companyThemeDTO, companyTheme, true);
                company.Theme = companyTheme;
                this.CompanyModel.RegisterSave(company, true);
                return new CompanyThemeDTO(company.Id, companyTheme);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("CompanyTheme.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLmsDTO"/>.
        /// </returns>
        public CompanyLmsDTO[] GetLMSHistoryByCompanyId(int companyId)
        {
            return this.LmsCompanyModel.GetAllByCompanyId(companyId).Select(x => new CompanyLmsDTO(x)).ToArray();
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyDTO"/>.
        /// </returns>
        public CompanyDTO Save(CompanyDTO dto)
        {
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var companyModel = this.CompanyModel;
                var companyLicenseModel = this.CompanyLicenseModel;
                var instance = (dto.companyId == 0)
                    ? null
                    : companyModel.GetOneById(dto.companyId).Value;

                instance = this.Convert(dto, instance);
                var isTransient = instance.IsTransient();
                companyModel.RegisterSave(instance, true);

                if (isTransient && dto.licenseVO != null)
                {
                    var user = this.UserModel.GetOneById(dto.licenseVO.createdBy).Value;

                    var license = instance.CurrentLicense ?? new CompanyLicense();
                    license.Company = instance;
                    var licenseIsTransient = license.IsTransient();
                    if (licenseIsTransient)
                    {
                        license.CreatedBy = user;
                        license.DateCreated = DateTime.Now;
                    }

                    license.ModifiedBy = user;
                    license.DateModified = DateTime.Now;
                    var expiryDate = dto.licenseVO.expiryDate.ConvertFromUnixTimeStamp();
                    license.ExpiryDate = expiryDate < DateTime.Now || expiryDate == DateTime.MinValue ? dto.licenseVO.isTrial ? DateTime.Now.AddDays(30) : DateTime.Now.AddYears(1) : expiryDate;
                    var start = dto.licenseVO.startDate.ConvertFromUnixTimeStamp();
                    license.DateStart = start < DateTime.Now || start == SqlDateTime.MinValue.Value ? DateTime.Now : dto.licenseVO.startDate.ConvertFromUnixTimeStamp();
                    license.LicenseStatus = this.GetLicenseStatus(dto.licenseVO);
                    license.TotalLicensesCount = dto.licenseVO.totalLicensesCount;
                    
                    license.TotalParticipantsCount = dto.licenseVO.totalParticipantsCount == 0 ? 100 : dto.licenseVO.totalParticipantsCount;
                    license.Domain = dto.licenseVO.domain;
                    license.LicenseNumber = Guid.NewGuid().ToString();

                    companyLicenseModel.RegisterSave(license);

                    if (licenseIsTransient)
                    {
                        instance.Licenses.Add(license);
                        companyModel.RegisterSave(instance, false);
                    }
                }

                if (isTransient && dto.lmsVO != null)
                {
                    var lms = new LmsCompany
                                  {
                                      AcPassword = dto.lmsVO.acPassword,
                                      AcServer = dto.lmsVO.acServer,
                                      AcUsername = dto.lmsVO.acUsername,
                                      CompanyId = instance.Id,
                                      CreatedBy = dto.lmsVO.createdBy,
                                      DateCreated = DateTime.Now,
                                      DateModified = DateTime.Now,
                                      LmsProvider = this.LmsProviderModel.GetOneByName(dto.lmsVO.lmsProvider).Value,
                                      ModifiedBy = this.UserModel.GetOneById(dto.lmsVO.modifiedBy).Value.Return(x => x.Id, dto.lmsVO.createdBy),
                                      ConsumerKey = Guid.NewGuid().ToString(),
                                      SharedSecret = Guid.NewGuid().ToString(),
                                      LmsDomain = dto.lmsVO.lmsDomain.RemoveHttpProtocolAndTrailingSlash(),
                                      PrimaryColor = dto.lmsVO.primaryColor,
                                      Title = dto.lmsVO.title,
                                      UseSSL = dto.lmsVO.lmsDomain.IsSSL(),
                                      UseUserFolder = dto.lmsVO.useUserFolder,
                                      CanRemoveMeeting = dto.lmsVO.canRemoveMeeting,
                                      CanEditMeeting = dto.lmsVO.canEditMeeting,
                                      IsSettingsVisible = dto.lmsVO.isSettingsVisible,
                                      EnableOfficeHours = dto.lmsVO.enableOfficeHours,
                                      EnableStudyGroups = dto.lmsVO.enableStudyGroups,
                                      EnableCourseMeetings = dto.lmsVO.enableCourseMeetings,
                                      ShowEGCHelp = dto.lmsVO.showEGCHelp,
                                      ShowLmsHelp = dto.lmsVO.showLmsHelp,
                                      AddPrefixToMeetingName = dto.lmsVO.addPrefixToMeetingName,
                                      UserFolderName = dto.lmsVO.userFolderName,
                                      EnableProxyToolMode = dto.lmsVO.enableProxyToolMode,
                                      ProxyToolSharedPassword = dto.lmsVO.proxyToolPassword
                                  };

                    LmsCompanyModel.RegisterSave(lms);

                    if (lms.LmsProvider.Id != (int)LmsProviderEnum.Canvas && !dto.lmsVO.enableProxyToolMode)
                    {
                        var lmsUser = new LmsUser
                                          {
                                              LmsCompany = lms,
                                              Username = dto.lmsVO.lmsAdmin,
                                              Password = dto.lmsVO.lmsAdminPassword,
                                              Token = dto.lmsVO.lmsAdminToken,
                                              UserId = "0"
                                          };
                        LmsUserModel.RegisterSave(lmsUser, true);
                        lms.AdminUser = lmsUser;
                        LmsCompanyModel.RegisterSave(lms);
                    }
                }

                if ((!dto.primaryContactId.HasValue || dto.primaryContactId == default(int)) && dto.primaryContactVO != null)
                {
                    bool passwordChanged, emailChanged;
                    var user = this.ProcessPrimaryContact(dto, instance, out passwordChanged, out emailChanged);
                    var isUserTransient = user.IsTransient();
                    user.Company = instance;
                    UserModel.RegisterSave(user);
                    IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
                    instance.PrimaryContact = user;
                    companyModel.RegisterSave(instance, true);
                    if (isUserTransient)
                    {
                        UserActivationModel model = this.UserActivationModel;
                        UserActivation userActivation;
                        if ((userActivation = model.GetLatestByUser(user.Id).Value) == null)
                        {
                            userActivation = new UserActivation
                            {
                                User = user,
                                ActivationCode = Guid.NewGuid().ToString(),
                                DateExpires = DateTime.Now.AddDays(7)
                            };
                            model.RegisterSave(userActivation);
                        }

                        var license = instance.Licenses.FirstOrDefault();
                        if (license.Return(x => x.LicenseStatus == CompanyLicenseStatus.Trial, false))
                        {
                            user.Status = UserStatus.Active;
                            UserModel.RegisterSave(user);
                            this.SendTrialEmail(user, userActivation.ActivationCode, instance);
                        }
                        else if (license.Return(x => x.LicenseStatus == CompanyLicenseStatus.Enterprise, false))
                        {
                            user.Status = UserStatus.Active;
                            UserModel.RegisterSave(user);
                            this.SendEnterpriseEmail(user, userActivation.ActivationCode, instance);
                        }
                        else
                        {
                            this.SendActivation(user);    
                        }
                    }
                    else if (passwordChanged || emailChanged)
                    {
                        UserActivationModel model = this.UserActivationModel;
                        UserActivation userActivation;
                        if ((userActivation = model.GetLatestByUser(user.Id).Value) == null)
                        {
                            userActivation = new UserActivation
                            {
                                User = user,
                                ActivationCode = Guid.NewGuid().ToString(),
                                DateExpires = DateTime.Now.AddDays(7)
                            };
                            model.RegisterSave(userActivation);
                        }

                        this.SendActivationLinkEmail(user.FirstName, user.Email, userActivation.ActivationCode);
                    }
                }
                else if (instance.PrimaryContact == null)
                {
                    foreach (var companyLicense in instance.Licenses)
                    {
                        companyLicenseModel.RegisterDelete(companyLicense);
                    }

                    companyModel.RegisterDelete(instance);
                    companyModel.Flush();
                    var errorRes = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "CompanyWithoutContact", "Company was created without primary contact");
                    throw new FaultException<Error>(errorRes, errorRes.errorMessage);
                }

                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Update, instance.Id, instance.Id);
                var dtoResult = new CompanyDTO(instance);
                var lmses = isTransient ? LmsCompanyModel.GetAllByCompanyId(instance.Id).ToList() : new List<LmsCompany>();
                dtoResult.lmsVO = new CompanyLmsDTO(lmses.FirstOrDefault());
                return dtoResult;
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Company.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The process contact.
        /// </summary>
        /// <param name="companyDTO">
        /// The company DTO. 
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="passwordChanged">
        /// The password Changed.
        /// </param>
        /// <param name="emailChanged">
        /// The email Changed.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        private User ProcessPrimaryContact(CompanyDTO companyDTO, Company instance, out bool passwordChanged, out bool emailChanged)
        {
            var userModel = this.UserModel;
            var entityDto = companyDTO.primaryContactVO;
            var result = instance.PrimaryContact ?? (userModel.GetOneById(entityDto.userId).Value ?? new User());
            bool isTransient = result.IsTransient();
            passwordChanged = false;
            emailChanged = !isTransient && !entityDto.email.Equals(result.Email, StringComparison.InvariantCultureIgnoreCase);
            result.Email = entityDto.email;
            result.FirstName = entityDto.firstName;
            result.LastName = entityDto.lastName;
            if (isTransient)
            {
                result.DateCreated = DateTime.Now;
            }

            result.DateModified = DateTime.Now;
            result.Status = !isTransient ? result.Status : UserStatus.Inactive;
            result.Language = this.LanguageModel.GetOneById(entityDto.languageId).Value;
            result.TimeZone = this.TimeZoneModel.GetOneById(entityDto.timeZoneId).Value;
            result.UserRole = this.UserRoleModel.GetOneById(entityDto.userRoleId).Value;
            result.CreatedBy = entityDto.createdBy.HasValue ? userModel.GetOneById(entityDto.createdBy.Value).Value : null;
            result.ModifiedBy = entityDto.modifiedBy.HasValue ? userModel.GetOneById(entityDto.modifiedBy.Value).Value : null;
            if (isTransient && string.IsNullOrWhiteSpace(entityDto.password))
            {
                entityDto.password = AuthenticationModel.CreateRandomPassword();
            }

            if (isTransient || (!string.IsNullOrWhiteSpace(entityDto.password) && !result.ValidatePassword(entityDto.password)))
            {
                passwordChanged = true;
                result.SetPassword(entityDto.password);
            }

            return result;
        }
        
        #endregion
    }
}