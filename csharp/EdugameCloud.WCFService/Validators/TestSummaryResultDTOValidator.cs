using EdugameCloud.Core.Domain.DTO;
using Esynctraining.Core.Enums;
using Esynctraining.FluentValidation;
using FluentValidation;

namespace EdugameCloud.WCFService.Validators
{
    public sealed class TestSummaryResultDTOValidator : AbstractValidator<TestSummaryResultDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultDTOValidator"/> class.
        /// </summary>
        public TestSummaryResultDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.testId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Test id is empty");
            this.RuleFor(model => model.acSessionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "ACSession id is empty");
        }
    }
}