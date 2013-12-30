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
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BuildVersionService : BaseService, IBuildVersionService
    {
        #region Properties

        /// <summary>
        ///     Gets the build version Model.
        /// </summary>
        private BuildVersionModel BuildVersionModel
        {
            get
            {
                return IoC.Resolve<BuildVersionModel>();
            }
        }

        /// <summary>
        ///     Gets the build version type model.
        /// </summary>
        private BuildVersionTypeModel BuildVersionTypeModel
        {
            get
            {
                return IoC.Resolve<BuildVersionTypeModel>();
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
            BuildVersion buildVersion;
            BuildVersionModel model = this.BuildVersionModel;
            if ((buildVersion = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION, 
                        ErrorsTexts.SessionError_Subject, 
                        ErrorsTexts.SessionError_NotFound));
            }
            else
            {
                model.RegisterDelete(buildVersion, true);
                this.UpdateCache<BuildVersionService>(x => x.GetAll());
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        ///     Gets all build versions.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<BuildVersionDTO> GetAll()
        {
            return new ServiceResponse<BuildVersionDTO>
                       {
                           objects =
                               this.BuildVersionModel.GetAll()
                                   .Select(x => new BuildVersionDTO(x))
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
        public ServiceResponse<BuildVersionDTO> GetById(int id)
        {
            var result = new ServiceResponse<BuildVersionDTO>();
            BuildVersion buildVersion;
            if ((buildVersion = this.BuildVersionModel.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION, 
                        ErrorsTexts.SessionError_Subject, 
                        ErrorsTexts.SessionError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new BuildVersionDTO(buildVersion);
            }

            return result;
        }

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="build">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<BuildVersionDTO> Save(BuildVersionDTO build)
        {
            var result = new ServiceResponse<BuildVersionDTO>();
            ValidationResult validationResult;
            if (this.IsValid(build, out validationResult))
            {
                BuildVersionModel sessionModel = this.BuildVersionModel;
                bool isTransient = build.buildVersionId == 0;
                BuildVersion buildInstance = isTransient ? null : sessionModel.GetOneById(build.buildVersionId).Value;
                buildInstance = this.ConvertDto(build, buildInstance);
                sessionModel.RegisterSave(buildInstance, true);
                this.UpdateCache<BuildVersionService>(x => x.GetAll());

                result.@object = new BuildVersionDTO(buildInstance);
                return result;
            }

            result = (ServiceResponse<BuildVersionDTO>)this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="buildDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="BuildVersion"/>.
        /// </returns>
        private BuildVersion ConvertDto(BuildVersionDTO buildDTO, BuildVersion instance)
        {
            instance = instance ?? new BuildVersion();
            instance.IsActive = buildDTO.isActive;
            instance.BuildNumber = buildDTO.buildNumber;
            instance.DateCreated = buildDTO.dateCreated == DateTime.MinValue ? DateTime.Now : buildDTO.dateCreated;
            instance.DateModified = buildDTO.dateModified == DateTime.MinValue ? DateTime.Now : buildDTO.dateModified;
            instance.DescriptionHTML = buildDTO.descriptionHTML;
            instance.DescriptionSmall = buildDTO.descriptionSmall;

            instance.Type = this.BuildVersionTypeModel.GetOneById(buildDTO.buildVersionTypeId).Value;
            instance.File = this.FileModel.GetOneById(buildDTO.fileId).Value;
            return instance;
        }

        #endregion
    }
}