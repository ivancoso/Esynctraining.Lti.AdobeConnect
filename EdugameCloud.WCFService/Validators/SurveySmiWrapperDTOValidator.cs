namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The Survey SMI Wrapper DTO validator.
    /// </summary>
    public class SurveySMIWrapperDTOValidator : AbstractValidator<SurveySMIWrapperDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveySMIWrapperDTOValidator"/> class.
        /// </summary>
        /// <param name="quizValidator">
        /// The quiz Validator.
        /// </param>
        /// <param name="smiValidator">
        /// The smi Validator.
        /// </param>
        public SurveySMIWrapperDTOValidator(IValidator<SurveyDTO> quizValidator, IValidator<SubModuleItemDTO> smiValidator)
        {
            this.RuleFor(model => model.SurveyDTO).SetValidator(quizValidator)
            .Must(x => x != null && x.surveyId == default(int)).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Quiz id is not empty (use save)");
            this.RuleFor(model => model.SmiDTO).SetValidator(smiValidator)
                .Must(x => x != null && x.subModuleItemId == default(int)).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sub module item id is not empty (use save for editing)");
        }

    }
}