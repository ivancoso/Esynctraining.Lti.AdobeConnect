namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The Question Type DTO validator.
    /// </summary>
    public sealed class QuestionTypeDTOValidator : AbstractValidator<QuestionTypeDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionTypeDTOValidator"/> class.
        /// </summary>
        public QuestionTypeDTOValidator()
        {
            this.RuleFor(model => model.type).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Type name is empty");
        }

    }
}