namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The SNProfileDTO validator.
    /// </summary>
    public class SNProfileDTOValidator : AbstractValidator<SNProfileDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileDTOValidator"/> class.
        /// </summary>
        public SNProfileDTOValidator()
        {
            this.RuleFor(model => model.subModuleItemId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sub module item id is empty");
            this.RuleFor(model => model.profileName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Profile name is empty");
            this.RuleFor(model => model.userName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "User name is empty");
            this.RuleFor(model => model.links).SetCollectionValidator(new SNLinkDTOValidator());
            this.RuleFor(model => model.services).SetCollectionValidator(new SNProfileSNServiceDTOValidator());
        }
    }
}