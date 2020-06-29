namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The TestResult DTO validator.
    /// </summary>
    public sealed class TestQuestionResultDTOValidator : AbstractValidator<TestQuestionResultDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestQuestionResultDTOValidator"/> class.
        /// </summary>
        public TestQuestionResultDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.questionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question id is empty");
            this.RuleFor(model => model.question).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question is empty");
            this.RuleFor(model => model.questionTypeId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Question type is empty");
        }
    }
}