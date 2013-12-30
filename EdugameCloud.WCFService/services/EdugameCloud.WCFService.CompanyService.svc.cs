// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
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
        /// Gets the Address model.
        /// </summary>
        private AddressModel AddressModel
        {
            get
            {
                return IoC.Resolve<AddressModel>();
            }
        }

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
        private CompanyLicenseModel CompanyLicenseModel
        {
            get
            {
                return IoC.Resolve<CompanyLicenseModel>();
            }
        }

        /// <summary>
        /// Gets the company model.
        /// </summary>
        private CountryModel CountryModel
        {
            get
            {
                return IoC.Resolve<CountryModel>();
            }
        }

        /// <summary>
        /// Gets the state model.
        /// </summary>
        private StateModel StateModel
        {
            get
            {
                return IoC.Resolve<StateModel>();
            }
        }

        /// <summary>
        /// Gets the authentication model.
        /// </summary>
        private AuthenticationModel AuthenticationModel
        {
            get
            {
                return IoC.Resolve<AuthenticationModel>();
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
                model.RegisterDelete(company, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<Company>(NotificationType.Delete, company.Id, company.Id);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        /// The get all.
        /// </summary>
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
                Company instance = (dto.companyId == 0)
                                              ? null
                                              : companyModel.GetOneById(dto.companyId).Value;
                instance = this.ConvertDto(dto, instance);
                bool isTransient = instance.IsTransient();
                companyModel.RegisterSave(instance, true);

                if (isTransient && dto.licenseVO != null)
                {
                    var user = this.UserModel.GetOneById(dto.licenseVO.createdBy).Value;

                    var license = new CompanyLicense
                                      {
                                          Company = instance,
                                          CreatedBy = user,
                                          ModifiedBy = user,
                                          DateCreated = DateTime.Now,
                                          DateModified = DateTime.Now,
                                          ExpiryDate = dto.licenseVO.isTrial ? DateTime.Now.AddDays(30) : DateTime.Now.AddYears(1),
                                          IsTrial = dto.licenseVO.isTrial,
                                          TotalLicensesCount = dto.licenseVO.totalLicensesCount,
                                          Domain = dto.licenseVO.domain,
                                          LicenseNumber = Guid.NewGuid().ToString()
                                      };

                    companyLicenseModel.RegisterSave(license);
                    instance.Licenses.Add(license);
                    companyModel.RegisterSave(instance, false);
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
                        var license = instance.Licenses.FirstOrDefault();
                        if (license.Return(x => x.IsTrial.HasValue && x.IsTrial.Value, false))
                        {
                            user.Status = UserStatus.Active;
                            UserModel.RegisterSave(user);
                            this.SendTrialEmail(user, dto.primaryContactVO.password, instance);
                        }
                        else
                        {
                            this.SendActivation(user);    
                        }
                    }
                    else if (passwordChanged || emailChanged)
                    {
                        this.SendPasswordEmail(user.FirstName, user.Email, dto.primaryContactVO.password);
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
                return response;
            }

            response = this.UpdateResult(response, result);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, response, string.Empty);
            return response;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="companyDto">
        /// The company DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="Company"/>.
        /// </returns>
        private Company ConvertDto(CompanyDTO companyDto, Company instance)
        {
            instance = instance ?? new Company();
            instance.CompanyName = companyDto.companyName;
            instance.Status = companyDto.isActive ? CompanyStatus.Active : CompanyStatus.Inactive;
            instance.DateCreated = (companyDto.dateCreated == DateTime.MinValue) ? DateTime.Now : companyDto.dateCreated;
            instance.DateModified = (companyDto.dateModified == DateTime.MinValue) ? DateTime.Now : companyDto.dateModified;
            if (companyDto.addressVO != null)
            {
                instance.Address = instance.Address ?? new Address();
                var addressVo = companyDto.addressVO;
                instance.Address.Address1 = addressVo.address1;
                instance.Address.Address2 = addressVo.address2;
                instance.Address.City = addressVo.city;
                instance.Address.DateCreated = addressVo.dateCreated.HasValue ? addressVo.dateCreated.Value : DateTime.Now;
                instance.Address.DateModified = DateTime.Now;
                instance.Address.Country = addressVo.countryId.HasValue
                                               ? this.CountryModel.GetOneById(addressVo.countryId.Value).Value
                                               : null;
                instance.Address.State = addressVo.stateId.HasValue
                                               ? this.StateModel.GetOneById(addressVo.stateId.Value).Value
                                               : null;
                instance.Address.Zip = addressVo.zip;
                this.AddressModel.RegisterSave(instance.Address);
            }

            instance.PrimaryContact = (companyDto.primaryContactId.HasValue && companyDto.primaryContactId != default(int)) ? this.UserModel.GetOneById(companyDto.primaryContactId.Value).Value : instance.PrimaryContact;
            return instance;
        }

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
            var entityDto = companyDTO.primaryContactVO;
            var result = instance.PrimaryContact ?? (this.UserModel.GetOneById(entityDto.userId).Value ?? new User());
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
            result.CreatedBy = entityDto.createdBy.HasValue ? this.UserModel.GetOneById(entityDto.createdBy.Value).Value : null;
            result.ModifiedBy = entityDto.modifiedBy.HasValue ? this.UserModel.GetOneById(entityDto.modifiedBy.Value).Value : null;
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