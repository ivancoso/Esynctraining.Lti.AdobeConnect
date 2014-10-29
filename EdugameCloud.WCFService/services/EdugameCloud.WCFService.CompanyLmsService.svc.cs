// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

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
        private CompanyLmsModel CompanyLmsModel
        {
            get
            {
                return IoC.Resolve<CompanyLmsModel>();
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
        private CompanyModel CompanyModel
        {
            get
            {
                return IoC.Resolve<CompanyModel>();
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
        /// The <see cref="ServiceResponse{T}"/>.
        /// </returns>
        public ServiceResponse<CompanyLmsDTO> Save(CompanyLmsDTO resultDto)
        {
            var result = new ServiceResponse<CompanyLmsDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                bool isTransient = resultDto.id == 0;
                CompanyLms entity = isTransient ? null : this.CompanyLmsModel.GetOneById(resultDto.id).Value;
                entity = this.ConvertDto(resultDto, entity);
                if (isTransient)
                {
                    entity.ConsumerKey = Guid.NewGuid().ToString();
                    entity.SharedSecret = Guid.NewGuid().ToString();
                }

                this.CompanyLmsModel.RegisterSave(entity);
                if (isTransient && entity.LmsProvider.Id == (int)LmsProviderEnum.BrainHoney)
                {
                    var lmsUser = new LmsUser
                    {
                        CompanyLms = entity,
                        Username = resultDto.lmsAdmin,
                        Password = resultDto.lmsAdminPassword,
                        UserId = 0
                    };
                    LmsUserModel.RegisterSave(lmsUser, true);
                    entity.AdminUser = lmsUser;
                    CompanyLmsModel.RegisterSave(entity);
                }

                result.@object = new CompanyLmsDTO(entity);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{T}"/>.
        /// </returns>
        public ServiceResponse<ConnectionInfoDTO> TestConnection(CompanyLmsDTO resultDto)
        {
            var result = new ServiceResponse<ConnectionInfoDTO>();

            result.@object = new ConnectionInfoDTO { status = "Connected successfully", info = "some info here..." };

            return result;
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
        /// The <see cref="CompanyLms"/>.
        /// </returns>
        private CompanyLms ConvertDto(CompanyLmsDTO dto, CompanyLms instance)
        {
            instance = instance ?? new CompanyLms();
            instance.AcPassword = dto.acPassword;
            instance.AcServer = dto.acServer;
            instance.AcUsername = dto.acUsername;
            instance.Company = this.CompanyModel.GetOneById(dto.companyId).Value;
            instance.ConsumerKey = dto.consumerKey;
            instance.CreatedBy = this.UserModel.GetOneById(dto.createdBy).Value;
            instance.DateCreated = dto.dateCreated;
            instance.DateModified = dto.dateModified;
            instance.LmsProvider = this.LmsProviderModel.GetOneByName(dto.lmsProvider).Value;
            instance.ModifiedBy = this.UserModel.GetOneById(dto.modifiedBy).Value;
            instance.SharedSecret = dto.sharedSecret;
            if (instance.IsTransient() && !string.IsNullOrWhiteSpace(dto.lmsDomain))
            {
                instance.LmsDomain = dto.lmsDomain;
            }

            return instance;
        }

        #endregion
    }
}