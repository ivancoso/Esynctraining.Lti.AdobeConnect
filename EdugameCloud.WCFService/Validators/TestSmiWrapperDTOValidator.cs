namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The Test SMI Wrapper DTO validator.
    /// </summary>
    public sealed class TestSMIWrapperDTOValidator : AbstractValidator<TestSMIWrapperDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestSMIWrapperDTOValidator"/> class.
        /// </summary>
        /// <param name="testValidator">
        /// The test Validator.
        /// </param>
        /// <param name="smiValidator">
        /// The smi Validator.
        /// </param>
        public TestSMIWrapperDTOValidator(IValidator<TestDTO> testValidator, IValidator<SubModuleItemDTO> smiValidator)
        {
            this.RuleFor(model => model.TestDTO).SetValidator(testValidator)
                .Must(x => x != null && x.testId == default(int)).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Test id is not empty (use save for editing)");
            this.RuleFor(model => model.SmiDTO).SetValidator(smiValidator)
                .Must(x => x != null && x.subModuleItemId == default(int)).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sub module item id is not empty (use save for editing)");
        }
    }
}