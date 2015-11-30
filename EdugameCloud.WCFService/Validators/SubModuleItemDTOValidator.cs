namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The SubModuleItem DTO validator.
    /// </summary>
    public sealed class SubModuleItemDTOValidator : AbstractValidator<SubModuleItemDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemDTOValidator"/> class.
        /// </summary>
        public SubModuleItemDTOValidator()
        {
            this.RuleFor(model => model.subModuleCategoryId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sub module category is empty");
        }
    }
}