// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Domain.Contracts;
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
    public class CompanyService : BaseService, ICompanyService
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
        private CompanyLmsModel CompanyLmsModel
        {
            get
            {
                return IoC.Resolve<CompanyLmsModel>();
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteById(int id)
        {
            var result = new ServiceResponse<int>();
            Company company;
            CompanyModel model = this.CompanyModel;
            if ((company = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
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
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Delete, company.Id, company.Id);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        /// Deletes company theme by id.
        /// </summary>
        /// <param name="id">
        /// The id
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{Guid}"/>.
        /// </returns>
        public ServiceResponse<Guid> DeleteThemeById(Guid id)
        {
            var result = new ServiceResponse<Guid>();
            CompanyTheme companyTheme;
            var model = this.CompanyThemeModel;
            if ((companyTheme = model.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                var companyModel = CompanyModel;
                var companies = companyModel.GetAllByCompanyThemeId(id);
                foreach (var company in companies)
                {
                    company.Theme = null;
                    companyModel.RegisterSave(company);
                }

                model.RegisterDelete(companyTheme, true);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse RequestLicenseUpgrade(int companyId)
        {
            var result = new ServiceResponse();
            Company company;
            if ((company = this.CompanyModel.GetOneById(companyId).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                this.SendLicenseUpgradeEmail(company);
            }

            return result;
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CompanyDTO> GetAll()
        {
            return new ServiceResponse<CompanyDTO>
                       {
                           objects =
                               this.CompanyModel.GetAll()
                                   .Select(x => new CompanyDTO(x))
                                   .ToList()
                       };
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CompanyLicenseDTO> GetLicenseHistoryByCompanyId(int companyId)
        {
            var result = new ServiceResponse<CompanyLicenseDTO>();
            Company company;
            if ((company = this.CompanyModel.GetOneById(companyId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@objects = company.Licenses.ToList().OrderByDescending(x => x.ExpiryDate).Select(x => new CompanyLicenseDTO(x)).ToList();
            }

            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CompanyDTO> GetById(int id)
        {
            var result = new ServiceResponse<CompanyDTO>();
            Company company;
            if ((company = this.CompanyModel.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new CompanyDTO(company);
            }

            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CompanyThemeDTO> GetThemeByCompanyId(int id)
        {
            var result = new ServiceResponse<CompanyThemeDTO>();
            Company company;
            if ((company = this.CompanyModel.GetOneById(id).Value) == null || (company.CurrentLicense ?? company.FutureActiveLicense).With(cl => cl.LicenseStatus != CompanyLicenseStatus.Enterprise))
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = company.Theme == null ? null : new CompanyThemeDTO(company.Id, company.Theme);
            }

            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CompanyThemeDTO> GetThemeById(Guid id)
        {
            var result = new ServiceResponse<CompanyThemeDTO>();
            CompanyTheme companyTheme;
            Company company;
            if ((companyTheme = this.CompanyThemeModel.GetOneById(id).Value) == null || ((company = this.CompanyModel.GetOneByCompanyThemeId(companyTheme.Id).Value) == null) || (company.CurrentLicense ?? company.FutureActiveLicense).With(cl => cl.LicenseStatus != CompanyLicenseStatus.Enterprise))
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new CompanyThemeDTO(company.Id, companyTheme);
            }

            return result;
        }

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="companyId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse ActivateById(int companyId)
        {
            var result = new ServiceResponse();
            var model = this.CompanyModel;
            Company company;
            if ((company = model.GetOneById(companyId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else
            {
                company.Status = CompanyStatus.Active;
                model.RegisterSave(company, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Update, company.Id, company.Id);
            }

            return result;
        }

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="companyId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse DeactivateById(int companyId)
        {
            var result = new ServiceResponse();
            var model = this.CompanyModel;
            Company company;
            if ((company = model.GetOneById(companyId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else
            {
                company.Status = CompanyStatus.Inactive;
                model.RegisterSave(company, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Update, company.Id, company.Id);
            }

            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="companyThemeDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CompanyThemeDTO> SaveTheme(CompanyThemeDTO companyThemeDTO)
        {
            var result = new ServiceResponse<CompanyThemeDTO>();
            ValidationResult validationResult;
            if (this.IsValid(companyThemeDTO, out validationResult))
            {
                var company = this.CompanyModel.GetOneById(companyThemeDTO.companyId).Value;
                var companyTheme = company.With(x => x.Theme);
                companyTheme = this.Convert(companyThemeDTO, companyTheme, true);
                company.Theme = companyTheme;
                this.CompanyModel.RegisterSave(company, true);
                result.@object = new CompanyThemeDTO(company.Id, companyTheme);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{T}"/>.
        /// </returns>
        public ServiceResponse<CompanyLmsDTO> GetLMSHistoryByCompanyId(int companyId)
        {
            return new ServiceResponse<CompanyLmsDTO>
            {
                objects =
                    this.CompanyLmsModel.GetAllByCompanyId(companyId)
                        .Select(x => new CompanyLmsDTO(x))
                        .ToList()
            };
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CompanyDTO> Save(CompanyDTO dto)
        {
            ValidationResult result;
            var response = new ServiceResponse<CompanyDTO>();
            if (this.IsValid(dto, out result))
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
                    license.ExpiryDate = dto.licenseVO.expiryDate < DateTime.Now || dto.licenseVO.expiryDate == DateTime.MinValue ? dto.licenseVO.isTrial ? DateTime.Now.AddDays(30) : DateTime.Now.AddYears(1) : dto.licenseVO.expiryDate.AdaptToSQL();
                    license.DateStart = dto.licenseVO.startDate < DateTime.Now || dto.licenseVO.startDate == DateTime.MinValue ? DateTime.Now : dto.licenseVO.startDate.AdaptToSQL();
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
                    var lms = new CompanyLms
                                  {
                                      AcPassword = dto.lmsVO.acPassword,
                                      AcServer = dto.lmsVO.acServer,
                                      AcUsername = dto.lmsVO.acUsername,
                                      Company = instance,
                                      CreatedBy = this.UserModel.GetOneById(dto.lmsVO.createdBy).Value,
                                      DateCreated = dto.lmsVO.dateCreated,
                                      DateModified = dto.lmsVO.dateModified,
                                      LmsProvider = this.LmsProviderModel.GetOneByName(dto.lmsVO.lmsProvider).Value,
                                      ModifiedBy = this.UserModel.GetOneById(dto.lmsVO.modifiedBy).Value,
                                      ConsumerKey = Guid.NewGuid().ToString(),
                                      SharedSecret = Guid.NewGuid().ToString(),
                                      LmsDomain = dto.lmsVO.lmsDomain,
                                      PrimaryColor = dto.lmsVO.primaryColor,
                                      Layout = dto.lmsVO.layout,
                                      Title = dto.lmsVO.title
                                  };

                    CompanyLmsModel.RegisterSave(lms);

                    if (lms.LmsProvider.Id == (int)LmsProviderEnum.BrainHoney || 
                        lms.LmsProvider.Id == (int)LmsProviderEnum.Moodle || 
                        lms.LmsProvider.Id == (int)LmsProviderEnum.Blackboard)
                    {
                        var lmsUser = new LmsUser
                                          {
                                              CompanyLms = lms,
                                              Username = dto.lmsVO.lmsAdmin,
                                              Password = dto.lmsVO.lmsAdminPassword,
                                              Token = dto.lmsVO.lmsAdminToken,
                                              UserId = "0"
                                          };
                        LmsUserModel.RegisterSave(lmsUser, true);
                        lms.AdminUser = lmsUser;
                        CompanyLmsModel.RegisterSave(lms);
                    }
                }

                if ((!dto.primaryContactId.HasValue || dto.primaryContactId == default(int)) && dto.primaryContactVO != null)
                {
                    bool passwordChanged, emailChanged;
                    var user = this.ProcessPrimaryContact(dto, instance, out passwordChanged, out emailChanged);
                    var isUserTransient = user.IsTransient();
                    user.Company = instance;
                    UserModel.RegisterSave(user);
                    IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
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
                    response.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "CompanyWithoutContact", "Company was created without primary contact"));
                    return response;
                }

                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Update, instance.Id, instance.Id);
                response.@object = new CompanyDTO(instance);

                var lmses = isTransient ? CompanyLmsModel.GetAllByCompanyId(instance.Id).ToList() : new List<CompanyLms>();
                response.@object.lmsVO = new CompanyLmsDTO(lmses.FirstOrDefault());
                return response;
            }

            response = this.UpdateResult(response, result);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, response, string.Empty);
            return response;
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
            result.DateCreated = entityDto.dateCreated == DateTime.MinValue ? DateTime.Now : entityDto.dateCreated;
            result.DateModified = entityDto.dateModified == DateTime.MinValue ? DateTime.Now : entityDto.dateModified;
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