namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The QuizResult DTO validator.
    /// </summary>
    public sealed class QuizResultDTOValidator : AbstractValidator<QuizResultDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultDTOValidator"/> class.
        /// </summary>
        /// <param name="quizResultModel">
        /// The quiz Result Model.
        /// </param>
        public QuizResultDTOValidator(QuizResultModel quizResultModel)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.participantName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Participant name is empty");
            this.RuleFor(model => model.email)
                .Must((model, x) => (!model.isArchive && string.IsNullOrWhiteSpace(x)) || (model.isArchive && !string.IsNullOrWhiteSpace(x)))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Email should be provided only in isArchive mode")
                .Must((model, x) => (!model.isArchive && string.IsNullOrWhiteSpace(x)) || (quizResultModel.GetOneByACSessionIdAndEmail(model.acSessionId, x).Value == null  && quizResultModel.GetOneByACSessionIdAndAcEmail(model.acSessionId, x).Value == null))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_SESSION, "You have already passed this quiz");
        }

    }
}