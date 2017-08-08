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

        private BuildVersionModel BuildVersionModel => IoC.Resolve<BuildVersionModel>();

        private BuildVersionTypeModel BuildVersionTypeModel => IoC.Resolve<BuildVersionTypeModel>();

        #endregion

        #region Public Methods and Operators

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

        public BuildVersionDTO[] GetAll()
        {
            return this.BuildVersionModel.GetAll().Select(x => new BuildVersionDTO(x)).ToArray();
        }

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