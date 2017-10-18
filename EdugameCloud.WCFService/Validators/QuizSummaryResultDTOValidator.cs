using EdugameCloud.Core.Domain.DTO;
using Esynctraining.Core.Enums;
using Esynctraining.FluentValidation;
using FluentValidation;

namespace EdugameCloud.WCFService.Validators
{

    /// <summary>
    /// The QuizSummaryResult DTO validator.
    /// </summary>
    public sealed class QuizSummaryResultDTOValidator : AbstractValidator<QuizSummaryResultDTO>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultDTOValidator"/> class.
        /// </summary>
        /// <param name="quizSummaryResultDTOModel">
        /// The quiz Result Model.
        /// </param>
        public QuizSummaryResultDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.quizId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Quiz id is empty");
            this.RuleFor(model => model.acSessionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "ACSession id is empty");
        }
    }
}