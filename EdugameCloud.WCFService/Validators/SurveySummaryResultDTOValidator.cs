using EdugameCloud.Core.Domain.DTO;
using Esynctraining.Core.Enums;
using Esynctraining.FluentValidation;
using FluentValidation;

namespace EdugameCloud.WCFService.Validators
{
    public sealed class SurveySummaryResultDTOValidator : AbstractValidator<SurveySummaryResultDTO>
    {
        public SurveySummaryResultDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.surveyId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Survey id is empty");
            this.RuleFor(model => model.acSessionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "ACSession id is empty");
        }
    }
}