// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

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
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteById(int id)
        {
            BuildVersion buildVersion;
            BuildVersionModel model = this.BuildVersionModel;
            if ((buildVersion = model.GetOneById(id).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION, 
                        ErrorsTexts.SessionError_Subject, 
                        ErrorsTexts.SessionError_NotFound);
                this.LogError("BuildVersion.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(buildVersion, true);
            this.UpdateCache<BuildVersionService>(x => x.GetAll());
            return id;
        }

        /// <summary>
        ///     Gets all build versions.
        /// </summary>
        /// <returns>
        ///     The <see cref="BuildVersionDTO" />.
        /// </returns>
        public BuildVersionDTO[] GetAll()
        {
            return this.BuildVersionModel.GetAll().Select(x => new BuildVersionDTO(x)).ToArray();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="BuildVersionDTO"/>.
        /// </returns>
        public BuildVersionDTO GetById(int id)
        {
            BuildVersion buildVersion;
            if ((buildVersion = this.BuildVersionModel.GetOneById(id).Value) == null)
            {
                var error = new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION, 
                        ErrorsTexts.SessionError_Subject, 
                        ErrorsTexts.SessionError_NotFound);
                this.LogError("BuildVersion.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new BuildVersionDTO(buildVersion);
        }

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="build">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="BuildVersionDTO"/>.
        /// </returns>
        public BuildVersionDTO Save(BuildVersionDTO build)
        {
            ValidationResult validationResult;
            if (this.IsValid(build, out validationResult))
            {
                BuildVersionModel sessionModel = this.BuildVersionModel;
                bool isTransient = build.buildVersionId == 0;
                BuildVersion buildInstance = isTransient ? null : sessionModel.GetOneById(build.buildVersionId).Value;
                buildInstance = this.ConvertDto(build, buildInstance);
                sessionModel.RegisterSave(buildInstance, true);
                this.UpdateCache<BuildVersionService>(x => x.GetAll());
                return new BuildVersionDTO(buildInstance);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("BuildVersion.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
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
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            instance.DescriptionHTML = buildDTO.descriptionHTML;
            instance.DescriptionSmall = buildDTO.descriptionSmall;

            instance.Type = this.BuildVersionTypeModel.GetOneById(buildDTO.buildVersionTypeId).Value;
            instance.File = this.FileModel.GetOneById(buildDTO.fileId).Value;
            return instance;
        }

        #endregion
    }
}