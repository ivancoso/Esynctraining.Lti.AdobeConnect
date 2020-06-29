namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;
    using FluentValidation;

    /// <summary>
    /// The SNMapSettingsDTO validator.
    /// </summary>
    public sealed class SNMapSettingsDTOValidator : AbstractValidator<SNMapSettingsDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapSettingsDTOValidator"/> class.
        /// </summary>
        public SNMapSettingsDTOValidator()
        {
        }
    }
}