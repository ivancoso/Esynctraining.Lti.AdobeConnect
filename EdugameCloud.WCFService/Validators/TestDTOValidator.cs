namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The Quiz DTO validator.
    /// </summary>
    public sealed class TestDTOValidator : AbstractValidator<TestDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestDTOValidator"/> class.
        /// </summary>
        public TestDTOValidator()
        {
            this.RuleFor(model => model.testName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Test name is empty");
        }
    }
}