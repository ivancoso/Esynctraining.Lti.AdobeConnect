namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The Theme DTO validator.
    /// </summary>
    public sealed class ThemeDTOValidator : AbstractValidator<ThemeDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeDTOValidator"/> class.
        /// </summary>
        public ThemeDTOValidator()
        {
            this.RuleFor(model => model.themeName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Theme name is empty");
        }

    }
}