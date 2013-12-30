namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using FluentValidation;

    /// <summary>
    /// The Survey QuestionResult Answer DTO validator.
    /// </summary>
    public class SurveyQuestionResultAnswerDTOValidator : AbstractValidator<SurveyQuestionResultAnswerDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultAnswerDTOValidator"/> class.
        /// </summary>
        public SurveyQuestionResultAnswerDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.surveyQuestionResultId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Survey question result id is empty");
            this.RuleFor(model => model.value).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Value is empty");
        }
    }
}