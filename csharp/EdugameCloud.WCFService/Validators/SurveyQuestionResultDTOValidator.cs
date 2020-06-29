namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;
    using FluentValidation;

    /// <summary>
    /// The Survey QuestionResult DTO validator.
    /// </summary>
    public sealed class SurveyQuestionResultDTOValidator : AbstractValidator<SurveyQuestionResultDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultDTOValidator"/> class.
        /// </summary>
        public SurveyQuestionResultDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.questionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question id is empty");
            this.RuleFor(model => model.question).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question is empty");
            this.RuleFor(model => model.questionTypeId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question type is empty");
        }
    }
}