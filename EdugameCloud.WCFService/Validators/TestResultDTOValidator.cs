namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The QuizResult DTO validator.
    /// </summary>
    public sealed class TestResultDTOValidator : AbstractValidator<TestResultDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultDTOValidator"/> class.
        /// </summary>
        /// <param name="testResultModel">
        /// The test Result Model.
        /// </param>
        public TestResultDTOValidator(TestResultModel testResultModel)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.testId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Test id is empty");
            this.RuleFor(model => model.acSessionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "ACSession id is empty");
            this.RuleFor(model => model.participantName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Participant name is empty");
            this.RuleFor(model => model.email)
                .Must((model, x) => (!model.isArchive && string.IsNullOrWhiteSpace(x)) || (model.isArchive && !string.IsNullOrWhiteSpace(x)))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Email should be provided only in isArchive mode")
                .Must((model, x) => (!model.isArchive && string.IsNullOrWhiteSpace(x)) || testResultModel.GetOneByACSessionIdAndEmail(model.acSessionId, x).Value == null)
                .WithError(Errors.CODE_ERRORTYPE_INVALID_SESSION, "You have already passed this test");
        }
    }
}