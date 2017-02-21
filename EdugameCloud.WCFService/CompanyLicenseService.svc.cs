// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Data.SqlTypes;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

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
        private CompanyLicenseModel CompanyLicenseModel => IoC.Resolve<CompanyLicenseModel>();

        private CompanyModel CompanyModel => IoC.Resolve<CompanyModel>();

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
            CompanyLicense companyLicense;
            var model = this.CompanyLicenseModel;
            if ((companyLicense = model.GetOneById(id).Value) == null)
            {
                var error = 
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("AppletResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }
            
            model.RegisterDelete(companyLicense, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<CompanyLicense>(NotificationType.Delete, companyLicense.Company.Id, companyLicense.Id);
            return id;
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="CompanyLicenseDTO"/>.
        /// </returns>
        public CompanyLicenseDTO[] GetAll()
        {
            return this.CompanyLicenseModel.GetAll().Select(x => new CompanyLicenseDTO(x)).ToArray();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLicenseDTO"/>.
        /// </returns>
        public CompanyLicenseDTO GetById(int id)
        {
            CompanyLicense companyLicense;
            if ((companyLicense = this.CompanyLicenseModel.GetOneById(id).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("CompanyLicense.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new CompanyLicenseDTO(companyLicense);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLicenseDTO"/>.
        /// </returns>
        public CompanyLicenseDTO Save(CompanyLicenseDTO dto)
        {
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var companyLicenseModel = this.CompanyLicenseModel;
                var instance = (dto.companyLicenseId == 0)
                                              ? null
                                              : companyLicenseModel.GetOneById(dto.companyLicenseId).Value;
                instance = this.ConvertDto(dto, instance);
                companyLicenseModel.RegisterSave(instance, true);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<CompanyLicense>(NotificationType.Update, instance.Company.Id, instance.Id);
                return new CompanyLicenseDTO(instance);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("CompanyLicense.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
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
        /// The <see cref="int"/>.
        /// </returns>
        public int UpdateSeatsCount(int licenseId, int seatsCount)
        {
            var model = this.CompanyLicenseModel;
            CompanyLicense companyLicense;
            if ((companyLicense = model.GetOneById(licenseId).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("CompanyLicense.UpdateSeatsCount", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            companyLicense.TotalLicensesCount = seatsCount;
            model.RegisterSave(companyLicense, true);
            return seatsCount;
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
            var expiryDate = licenseDto.expiryDate.ConvertFromUnixTimeStamp();
            instance.ExpiryDate = expiryDate <= SqlDateTime.MinValue.Value ? licenseDto.isTrial ? DateTime.Now.AddDays(30) : DateTime.Now.AddYears(1) : expiryDate;
            instance.DateStart = licenseDto.startDate.ConvertFromUnixTimeStamp();
            instance.TotalLicensesCount = licenseDto.totalLicensesCount;
            instance.TotalParticipantsCount = licenseDto.totalParticipantsCount == 0 ? 100 : licenseDto.totalParticipantsCount;
            instance.DateModified = DateTime.Now;
            instance.ModifiedBy = this.UserModel.GetOneById(licenseDto.modifiedBy).Value;
            instance.LicenseStatus = this.GetLicenseStatus(licenseDto);
            instance.HasApi = licenseDto.hasApi;

            return instance;
        }

        #endregion
    }
}