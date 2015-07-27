namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The QuizResult DTO validator.
    /// </summary>
    public sealed class QuizQuestionResultDTOValidator : AbstractValidator<QuizQuestionResultDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuizQuestionResultDTOValidator"/> class.
        /// </summary>
        public QuizQuestionResultDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.quizResultId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Quiz result id is empty");
            this.RuleFor(model => model.questionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question id is empty");
            this.RuleFor(model => model.question).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question is empty");
            this.RuleFor(model => model.questionTypeId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question type is empty");

        }

    }
}