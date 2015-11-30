namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;
    using FluentValidation;

    /// <summary>
    /// The DistractorDTO validator.
    /// </summary>
    public sealed class DistractorDTOValidator : AbstractValidator<DistractorDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistractorDTOValidator"/> class.
        /// </summary>
        public DistractorDTOValidator()
        {
            this.RuleFor(model => model.distractor).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Distractor text is empty");
        }
    }
}