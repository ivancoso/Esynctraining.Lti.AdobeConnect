namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
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

    using NHibernate.Driver;

    using Resources;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class CompanyLmsService : BaseService, ICompanyLmsService
    {
        #region Properties

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
        /// Gets the company model.
        /// </summary>
        private LmsProviderModel LmsProviderModel
        {
            get
            {
                return IoC.Resolve<LmsProviderModel>();
            }
        }

        /// <summary>
        /// Gets the user model.
        /// </summary>
        private UserModel UserModel
        {
            get
            {
                return IoC.Resolve<UserModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The save.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse{T}"/>.
        /// </returns>
        public ServiceResponse<CompanyLmsDTO> Save(CompanyLmsDTO resultDto)
        {
            var result = new ServiceResponse<CompanyLmsDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var isTransient = resultDto.id == 0;
                var entity = isTransient ? null : CompanyLmsModel.GetOneById(resultDto.id).Value;
                entity = ConvertDto(resultDto, entity);
                if (isTransient)
                {
                    entity.ConsumerKey = Guid.NewGuid().ToString();
                    entity.SharedSecret = Guid.NewGuid().ToString();
                }

                CompanyLmsModel.RegisterSave(entity);
                result.@object = new CompanyLmsDTO(entity);
                return result;
            }

            result = (ServiceResponse<CompanyLmsDTO>)this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse{T}"/>.
        /// </returns>
        public ServiceResponse<ConnectionInfoDTO> TestConnection(CompanyLmsDTO resultDto)
        {
            var result = new ServiceResponse<ConnectionInfoDTO>();
            
            result.@object = new ConnectionInfoDTO()
                             {
                                 status = "Connected successfully",
                                 info = "some info here..."
                             };
            
            return result;
        }

        #endregion

        #region methods

        private CompanyLms ConvertDto(CompanyLmsDTO dto, CompanyLms instance)
        {
            instance = instance ?? new CompanyLms();
            instance.AcPassword = dto.acPassword;
            instance.AcServer = dto.acServer;
            instance.AcUsername = dto.acUsername;
            instance.Company = CompanyModel.GetOneById(dto.companyId).Value;
            instance.ConsumerKey = dto.consumerKey;
            instance.CreatedBy = UserModel.GetOneById(dto.createdBy).Value;
            instance.DateCreated = dto.dateCreated;
            instance.DateModified = dto.dateModified;
            instance.LmsProvider = LmsProviderModel.GetOneByName(dto.lmsProvider);
            instance.ModifiedBy = UserModel.GetOneById(dto.modifiedBy).Value;
            instance.SharedSecret = dto.sharedSecret;

            return instance;
        }

        #endregion
    }
}
