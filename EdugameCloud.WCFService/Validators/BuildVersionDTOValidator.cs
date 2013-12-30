namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The BuildVersion DTO validator.
    /// </summary>
    public class BuildVersionDTOValidator : AbstractValidator<BuildVersionDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionDTOValidator"/> class.
        /// </summary>
        public BuildVersionDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.buildNumber).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Build number is empty");
            this.RuleFor(model => model.fileId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "File id is empty");
            this.RuleFor(model => model.buildVersionTypeId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Type is empty");
        }

    }
}