namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The Survey DTO validator.
    /// </summary>
    public sealed class SurveyDTOValidator : AbstractValidator<SurveyDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyDTOValidator"/> class.
        /// </summary>
        public SurveyDTOValidator()
        {
            this.RuleFor(model => model.surveyName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Survey name is empty");
            this.RuleFor(model => model.surveyGroupingTypeId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Survey grouping type id is empty");
        }
    }
}