namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The SubModuleCategory DTO validator.
    /// </summary>
    public sealed class SubModuleCategoryDTOValidator : AbstractValidator<SubModuleCategoryDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleCategoryDTOValidator"/> class.
        /// </summary>
        public SubModuleCategoryDTOValidator()
        {
            this.RuleFor(model => model.userId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "User is empty");
            this.RuleFor(model => model.subModuleId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sub module is empty");
            this.RuleFor(model => model.categoryName).Must(x => string.IsNullOrWhiteSpace(x) || x.Return(s => s.Length <= 255, true)).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Category name should be less then 255");
        }

    }
}