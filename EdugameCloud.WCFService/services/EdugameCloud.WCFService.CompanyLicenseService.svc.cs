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
    public class CompanyLicenseService : BaseService, ICompanyLicenseService
    {
        #region Properties

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
        private CompanyModel CompanyModel
        {
            get
            {
                return IoC.Resolve<CompanyModel>();
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
            CompanyLicense companyLicense;
            var model = this.CompanyLicenseModel;
            if ((companyLicense = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(companyLicense, true);
                IoC.Resolve<RTMPModel>()
                   .NotifyClientsAboutChangesInTable<CompanyLicense>(NotificationType.Delete, companyLicense.Company.Id, companyLicense.Id);
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
        public ServiceResponse<CompanyLicenseDTO> GetAll()
        {
            return new ServiceResponse<CompanyLicenseDTO>
                       {
                           objects =
                               this.CompanyLicenseModel.GetAll()
                                   .Select(x => new CompanyLicenseDTO(x))
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
        public ServiceResponse<CompanyLicenseDTO> GetById(int id)
        {
            var result = new ServiceResponse<CompanyLicenseDTO>();
            CompanyLicense companyLicense;
            if ((companyLicense = this.CompanyLicenseModel.GetOneById(id).Value) == null)
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
                result.@object = new CompanyLicenseDTO(companyLicense);
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
        public ServiceResponse<CompanyLicenseDTO> Save(CompanyLicenseDTO dto)
        {
            ValidationResult result;
            var response = new ServiceResponse<CompanyLicenseDTO>();
            if (this.IsValid(dto, out result))
            {
                var companyLicenseModel = this.CompanyLicenseModel;
                var instance = (dto.companyLicenseId == 0)
                                              ? null
                                              : companyLicenseModel.GetOneById(dto.companyLicenseId).Value;
                instance = this.ConvertDto(dto, instance);
                companyLicenseModel.RegisterSave(instance, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<CompanyLicense>(NotificationType.Update, instance.Company.Id, instance.Id);
                response.@object = new CompanyLicenseDTO(instance);
                return response;
            }

            response = this.UpdateResult(response, result);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, response, string.Empty);
            return response;
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="licenseId">
        /// The license Id.
        /// </param>
        /// <param name="seatsCount">
        /// The seats Count.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> UpdateSeatsCount(int licenseId, int seatsCount)
        {
            var result = new ServiceResponse<int>();
            var model = this.CompanyLicenseModel;
            CompanyLicense companyLicense;
            if ((companyLicense = model.GetOneById(licenseId).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                companyLicense.TotalLicensesCount = seatsCount;
                model.RegisterSave(companyLicense, true);
                result.@object = seatsCount;
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="licenseDto">
        /// The license DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLicense"/>.
        /// </returns>
        private CompanyLicense ConvertDto(CompanyLicenseDTO licenseDto, CompanyLicense instance)
        {
            instance = instance ?? new CompanyLicense();
            var licenseIsTransient = instance.IsTransient();
            if (licenseIsTransient)
            {
                instance.CreatedBy = this.UserModel.GetOneById(licenseDto.createdBy).Value;
                instance.DateCreated = DateTime.Now;
                instance.Company = this.CompanyModel.GetOneById(licenseDto.companyId).Value;
                instance.LicenseNumber = Guid.NewGuid().ToString();
            }

            instance.Domain = licenseDto.domain;
            instance.ExpiryDate = licenseDto.expiryDate == DateTime.MinValue ? licenseDto.isTrial ? DateTime.Now.AddDays(30) : DateTime.Now.AddYears(1) : licenseDto.expiryDate.AdaptToSQL();
            instance.DateStart = licenseDto.startDate.AdaptToSQL();
            instance.TotalLicensesCount = licenseDto.totalLicensesCount;
            instance.TotalParticipantsCount = licenseDto.totalParticipantsCount == 0 ? 100 : licenseDto.totalParticipantsCount;
            instance.DateModified = DateTime.Now;
            instance.ModifiedBy = this.UserModel.GetOneById(licenseDto.modifiedBy).Value;
            instance.LicenseStatus = this.GetLicenseStatus(licenseDto);
            
            return instance;
        }

        #endregion
    }
}