namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The Quiz DTO validator.
    /// </summary>
    public sealed class QuizDTOValidator : AbstractValidator<QuizDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuizDTOValidator"/> class.
        /// </summary>
        public QuizDTOValidator()
        {
            this.RuleFor(model => model.quizName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Quiz name is empty");
//            this.RuleFor(model => model.quizFormatId).NotEqual(0).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Quiz format id is empty");
//            this.RuleFor(model => model.scoreTypeId).NotEqual(0).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Score type id is empty");
        }

    }
}