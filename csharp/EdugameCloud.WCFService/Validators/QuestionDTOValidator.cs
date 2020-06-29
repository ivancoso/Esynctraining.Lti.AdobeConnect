namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The Question DTO validator.
    /// </summary>
    public sealed class QuestionDTOValidator : AbstractValidator<QuestionDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionDTOValidator"/> class.
        /// </summary>
        public QuestionDTOValidator()
        {
            this.RuleFor(model => model.questionTypeId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question type id is empty");
            this.RuleFor(model => model.question).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question is empty");
        }

    }
}