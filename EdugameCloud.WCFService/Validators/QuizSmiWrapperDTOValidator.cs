namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The Quiz SMI Wrapper DTO validator.
    /// </summary>
    public class QuizSMIWrapperDTOValidator : AbstractValidator<QuizSMIWrapperDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuizSMIWrapperDTOValidator"/> class.
        /// </summary>
        /// <param name="quizValidator">
        /// The quiz Validator.
        /// </param>
        /// <param name="smiValidator">
        /// The smi Validator.
        /// </param>
        public QuizSMIWrapperDTOValidator(IValidator<QuizDTO> quizValidator, IValidator<SubModuleItemDTO> smiValidator)
        {
            this.RuleFor(model => model.QuizDTO).SetValidator(quizValidator)
                .Must(x => x != null && x.quizId == default(int)).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Quiz id is not empty (use save for editing)");
            this.RuleFor(model => model.SmiDTO).SetValidator(smiValidator)
                .Must(x => x != null && x.subModuleItemId == default(int)).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sub module item id is not empty (use save for editing)");

        }

    }
}